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
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private IHttpContextAccessor _httpContextAccessor;
        private string? _currentUserId;
        public UserService(ICrudRepository<User> userRepo, IPasswordHasher passwordHasher, IHttpContextAccessor ctx, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepo;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = ctx;
            _unitOfWork = unitOfWork;
            _currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
        }
        public string GetCurrentUserId()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            if (currentUserId is null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            return currentUserId;
        }
        public string GetCurrentUserExternalId()
        {
            throw new NotImplementedException();
        }
        public async Task<Result<User?>> GetUserByEmail(string email)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var user = users.FirstOrDefault(u => u.Email == email);
                if (user is null) { throw new ArgumentNullException("user not found"); }
                return Result<User?>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User?>.Failure($"An error occurred while retrieving the user: {ex.Message}");
            }
        }
        public async Task<Result<User?>> GetUserById(Guid id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user is null) { throw new ArgumentNullException("user not found"); }
                return Result<User?>.Success(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the user: {ex.Message}");
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
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                var result = await GetUserByEmail(user.Email);
                if (result.IsSuccess && result.Data is not null) throw new Exception("user with this email already exists");
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
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

        public async Task<Result<User>> DeleteUser(Guid id)
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

        public async Task<Result<User>> UpdateUser(Guid id, User updatedUser)
        {
            try
            {
                if (_currentUserId is not null && id.ToString() == _currentUserId!)
                {


                    var user = await _userRepository.GetByIdAsync(id);
                    if (user == null) throw new Exception("user was not found");
                    _userRepository.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    return Result<User>.Success(user);
                }
                else { return Result<User>.Failure("You can update only your own profile"); }
            }
            catch (Exception ex) { return Result<User>.Failure($"An error occurred while deleting the user: {ex.Message}"); }

        }
    }
}
