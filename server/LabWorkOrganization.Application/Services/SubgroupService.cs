using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.SubGroupDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class SubgroupService : ISubgroupService
    {
        private readonly ICourseScopedRepository<SubGroup> _subGroupRepository;
        private readonly ICrudRepository<QueuePlace> _queuePlaceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;

        public SubgroupService(
            ICourseScopedRepository<SubGroup> subGroupRepository,
            ICrudRepository<QueuePlace> queuePlaceRepository,
            IUnitOfWork unitOfWork,
            IUserService userService,
            ICourseService courseService)
        {
            _subGroupRepository = subGroupRepository;
            _queuePlaceRepository = queuePlaceRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _courseService = courseService;
        }

        private async Task EnsureCurrentUserIsOwnerOfCourse(string courseId)
        {
            var currentUserId = _userService.GetCurrentUserId();
            var course = await _courseService.GetCourseById(courseId);
            if (!course.IsSuccess || course.Data is null)
                throw new Exception("Course not found");

            if (course.Data.OwnerId != currentUserId)
                throw new UnauthorizedAccessException("You are not the owner of this course");
        }

        public async Task<Result<SubGroup>> CreateSubgroup(SubGroupCreationalDto subgroup)
        {
            try
            {
                await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);

                var validationErrors = Validation.ValidationHelper.Validate(subgroup);
                if (validationErrors.Any())
                    throw new ArgumentException(string.Join("; ", validationErrors));

                var courseResult = await _courseService.GetCourseById(subgroup.CourseId);
                if (!courseResult.IsSuccess || courseResult.Data is null)
                    throw new Exception(courseResult.ErrorMessage);

                // тільки зареєстровані юзери
                var studentList = new List<User>();
                foreach (var email in subgroup.StudentsEmails)
                {
                    var userResult = await _userService.GetUserByEmail(email);
                    if (userResult.IsSuccess && userResult.Data is not null)
                        studentList.Add(userResult.Data);
                }

                var newSubgroup = new SubGroup
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = subgroup.Name,
                    CourseId = subgroup.CourseId,
                    AllowedDays = subgroup.AllowedDays,
                    Students = studentList
                };

                await _subGroupRepository.AddAsync(newSubgroup);
                await _unitOfWork.SaveChangesAsync();

                return Result<SubGroup>.Success(newSubgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"Error creating subgroup: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> DeleteSubgroup(string subgroupId)
        {
            try
            {
                var subgroup = await _subGroupRepository.GetByIdAsync(subgroupId);
                if (subgroup is null)
                    throw new Exception("Subgroup not found");

                await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);

                _subGroupRepository.Delete(subgroup);
                await _unitOfWork.SaveChangesAsync();

                return Result<SubGroup>.Success(subgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"Error deleting subgroup: {ex.Message}");
            }
        }

        // 🔥 головний метод — отримати всі підгрупи по courseId
        public async Task<Result<IEnumerable<SubGroup>>> GetAllSubgroupsByCourseId(string courseId)
        {
            try
            {
                var subgroups = await _subGroupRepository.GetAllByCourseIdAsync(courseId) ?? new List<SubGroup>();
                return Result<IEnumerable<SubGroup>>.Success(subgroups);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<SubGroup>>.Failure($"Error retrieving subgroups: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> UpdateStudents(SubGroupStudentsDto subGroupStudents)
        {
            try
            {
                var subgroup = await _subGroupRepository.GetByIdAsync(subGroupStudents.SubGroupId);
                if (subgroup is null)
                    throw new Exception("Subgroup not found");

                await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);

                var updatedStudents = new List<User>();
                foreach (var email in subGroupStudents.StudentsEmails)
                {
                    var userResult = await _userService.GetUserByEmail(email);
                    if (userResult.IsSuccess && userResult.Data is not null)
                        updatedStudents.Add(userResult.Data);
                }

                subgroup.Students = updatedStudents;
                _subGroupRepository.Update(subgroup);
                await _unitOfWork.SaveChangesAsync();

                return Result<SubGroup>.Success(subgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"Error updating subgroup: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> AddToQueue(QueuePlaceCreationalDto queuePlace)
        {
            try
            {
                var subgroup = await _subGroupRepository.GetByIdAsync(queuePlace.SubGroupId);
                if (subgroup is null)
                    throw new Exception("Subgroup not found");

                await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);

                subgroup.Queue.Add(new QueuePlace
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = queuePlace.UserId,
                    SubGroupId = queuePlace.SubGroupId,
                    SpecifiedTime = queuePlace.SpecifiedTime
                });

                await _unitOfWork.SaveChangesAsync();
                return Result<SubGroup>.Success(subgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"Error adding to queue: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> RemoveFromQueue(string queuePlaceId)
        {
            try
            {
                var queuePlace = await _queuePlaceRepository.GetByIdAsync(queuePlaceId);
                if (queuePlace is null)
                    throw new Exception("Queue place not found");

                var subgroup = await _subGroupRepository.GetByIdAsync(queuePlace.SubGroupId);
                if (subgroup is null)
                    throw new Exception("Subgroup not found");

                await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);

                subgroup.Queue.Remove(queuePlace);
                _queuePlaceRepository.Delete(queuePlace);
                await _unitOfWork.SaveChangesAsync();

                return Result<SubGroup>.Success(subgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"Error removing from queue: {ex.Message}");
            }
        }
    }
}
