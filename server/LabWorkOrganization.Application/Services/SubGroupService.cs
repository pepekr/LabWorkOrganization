using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class SubGroupService
    {
        private readonly ICrudRepository<SubGroup> _crudRepository;
        private readonly ICrudRepository<User> _userRepository;
        private readonly ICrudRepository<Course> _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubGroupService(
            ICrudRepository<SubGroup> crudRepository,
            ICrudRepository<User> userRepository,
            ICrudRepository<Course> courseRepository,
            IUnitOfWork unitOfWork)
        {
            _crudRepository = crudRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SubGroup>> CreateSubGroup(SubGroupCreationalDto dto)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(dto);
                if (errors.Count > 0)
                    throw new ArgumentException(string.Join("; ", errors));

                // Load users
                var users = new List<User>();
                foreach (var userId in dto.Students)
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user != null)
                        users.Add(user);
                }

                // Load course
                var course = await _courseRepository.GetByIdAsync(dto.CourseId);
                if (course == null)
                    return Result<SubGroup>.Failure("Course not found");

                var subGroup = new SubGroup
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    AllowedDays = dto.AllowedDays,
                    Students = users,
                    CourseId = dto.CourseId,
                    Course = course
                };

                await _crudRepository.AddAsync(subGroup);
                await _unitOfWork.SaveChangesAsync();

                return Result<SubGroup>.Success(subGroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"An error occurred while creating the subgroup: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup?>> GetById(Guid id)
        {
            try
            {
                var subGroup = await _crudRepository.GetByIdAsync(id);
                return subGroup != null
                    ? Result<SubGroup?>.Success(subGroup)
                    : Result<SubGroup?>.Failure("SubGroup not found");
            }
            catch (Exception ex)
            {
                return Result<SubGroup?>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<SubGroup>>> GetAll()
        {
            try
            {
                var subGroups = await _crudRepository.GetAllAsync() ?? Enumerable.Empty<SubGroup>();
                return Result<IEnumerable<SubGroup>>.Success(subGroups);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<SubGroup>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> UpdateSubGroup(SubGroup subGroup)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(subGroup);
                if (errors.Count > 0)
                    throw new ArgumentException(string.Join("; ", errors));

                _crudRepository.Update(subGroup);
                await _unitOfWork.SaveChangesAsync();

                return Result<SubGroup>.Success(subGroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"An error occurred while updating the subgroup: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> DeleteSubGroup(Guid id)
        {
            try
            {
                var subGroup = await _crudRepository.GetByIdAsync(id);
                if (subGroup == null)
                    return Result<SubGroup>.Failure("SubGroup not found");

                _crudRepository.Delete(subGroup);
                await _unitOfWork.SaveChangesAsync();

                return Result<SubGroup>.Success(subGroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"An error occurred while deleting the subgroup: {ex.Message}");
            }
        }
    }
}
