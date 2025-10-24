using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Http;

namespace LabWorkOrganization.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ICrudRepository<User> _userRepository;
        private readonly ICourseScopedRepository<SubGroup> _subGroupRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IExternalTokenService _externalTokenService;
        private readonly IExternalCrudRepoFactory _externalCrudFactory;

        public UserService(
            ICrudRepository<User> userRepo,
            IPasswordHasher passwordHasher,
            IHttpContextAccessor ctx,
            IUnitOfWork unitOfWork,
            ICourseScopedRepository<SubGroup> subGroupRepository,
            IExternalTokenService externalTokenService,
            IExternalCrudRepoFactory externalCrudFactory)
        {
            _userRepository = userRepo;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = ctx;
            _unitOfWork = unitOfWork;
            _subGroupRepository = subGroupRepository;
            _externalTokenService = externalTokenService;
            _externalCrudFactory = externalCrudFactory;
        }

        public string GetCurrentUserId()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            if (currentUserId is null)
                throw new UnauthorizedAccessException("User is not authenticated.");
            return currentUserId;
        }

        public string GetCurrentUserExternalId()
        {
            var externalId = _httpContextAccessor.HttpContext?.User?.FindFirst("external_id")?.Value;
            if (externalId is null)
                throw new UnauthorizedAccessException("External ID not found.");
            return externalId;
        }

        public async Task<Result<User?>> GetUserByEmail(string email)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var user = users.FirstOrDefault(u => u.Email == email);
                if (user is null)
                    throw new ArgumentNullException("user not found");
                return Result<User?>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User?>.Failure($"An error occurred while retrieving the user: {ex.Message}");
            }
        }

        public async Task<Result<User?>> GetUserById(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user is null)
                    throw new ArgumentNullException("user not found");
                return Result<User?>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User?>.Failure($"An error occurred while retrieving the user: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllAsync() ?? new List<User>();
                return Result<IEnumerable<User>>.Success(users);
            }
            catch (Exception)
            {
                return Result<IEnumerable<User>>.Failure("An error occurred while retrieving users");
            }
        }

        public async Task<Result<User>> AddUser(UserRegisterDto user)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(user);
                if (errors.Count > 0)
                    throw new ArgumentException(string.Join("; ", errors));

                var result = await GetUserByEmail(user.Email);
                if (result.IsSuccess && result.Data is not null)
                    throw new Exception("user with this email already exists");

                var newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = user.Email,
                    Name = user.Name,
                    HashedPassword = _passwordHasher.HashPassword(user.Password)
                };

                await _userRepository.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();
                return Result<User>.Success(newUser);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"An error occurred while creating the user: {ex.Message}");
            }
        }

        public async Task<Result<User>> DeleteUser(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) throw new Exception("user was not found");
                _userRepository.Delete(user);
                await _unitOfWork.SaveChangesAsync();
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"An error occurred while deleting the user: {ex.Message}");
            }
        }

        public async Task<Result<User>> UpdateUser(string id, User updatedUser)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (id != currentUserId)
                    return Result<User>.Failure("You can update only your own profile");

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new Exception("user was not found");

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"An error occurred while updating the user: {ex.Message}");
            }
        }

        // 🔥 Основна штука — прапорець external
        public async Task<IEnumerable<User>> GetAllUsersByCourseId(string courseId, bool external = false)
        {
            var result = new List<User>();

            try
            {
                if (!external)
                {
                    // 1️⃣ локальний курс → витягуємо студентів через підгрупи
                    var subgroups = await _subGroupRepository.GetAllByCourseIdAsync(courseId) ?? new List<SubGroup>();
                    var users = subgroups.SelectMany(sg => sg.Students).Distinct().ToList();
                    return users;
                }

                // 2️⃣ зовнішній курс → Google Classroom
                var userId = GetCurrentUserId();
                var accessTokenResult = await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                if (!accessTokenResult.IsSuccess)
                    throw new Exception(accessTokenResult.ErrorMessage);

                var repo = _externalCrudFactory.Create<User>(
                    $"https://classroom.googleapis.com/v1/courses/{courseId}/students"
                );

                var externalUsers = await repo.GetAllAsync() ?? Enumerable.Empty<User>();
                var localUsers = await _userRepository.GetAllAsync() ?? new List<User>();

                foreach (var externalUser in externalUsers)
                {
                    var local = localUsers.FirstOrDefault(u =>
                        (!string.IsNullOrEmpty(u.SubGoogleId) && u.SubGoogleId == externalUser.SubGoogleId) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email == externalUser.Email)
                    );

                    if (local != null)
                        result.Add(local);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users for course {courseId}: {ex.Message}");
                return result;
            }
        }
    }
}
