using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.SubGroupDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Validation;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class SubgroupService : ISubgroupService
    {
        private readonly ICourseService _courseService;
        private readonly ICrudRepository<QueuePlace> _queuePlaceRepository;
        private readonly ICourseScopedRepository<SubGroup> _subGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

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

        public async Task<Result<SubGroup>> CreateSubgroup(SubGroupCreationalDto subgroup)
        {
            try
            {
                await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);

                List<string> validationErrors = ValidationHelper.Validate(subgroup);
                if (validationErrors.Any())
                {
                    throw new ArgumentException(string.Join("; ", validationErrors));
                }

                Result<Course?> courseResult = await _courseService.GetCourseById(subgroup.CourseId);
                if (!courseResult.IsSuccess || courseResult.Data is null)
                {
                    throw new Exception(courseResult.ErrorMessage);
                }

                // тільки зареєстровані юзери
                List<User> studentList = new();
                foreach (string email in subgroup.StudentsEmails)
                {
                    Result<User?> userResult = await _userService.GetUserByEmail(email);
                    if (userResult.IsSuccess && userResult.Data is not null)
                    {
                        studentList.Add(userResult.Data);
                    }
                }

                SubGroup newSubgroup = new()
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
                SubGroup? subgroup = await _subGroupRepository.GetByIdAsync(subgroupId);
                if (subgroup is null)
                {
                    throw new Exception("Subgroup not found");
                }

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
        
        public async Task<Result<IEnumerable<SubGroupDto>>> GetAllSubgroupsByCourseId(string courseId)
        {
            try
            {
                // MODIFIED: Include Queue and related data
                IEnumerable<SubGroup> subgroups = await _subGroupRepository.GetAllByCourseIdAsync(courseId,
                    sg => sg.Students,
                    sg => sg.Queue
                );
                
                List<SubGroupDto> subgroupsResult = new List<SubGroupDto>();
                foreach (SubGroup subgroup in subgroups)
                {
                    SubGroupDto tempSubgroup = new();
                    tempSubgroup.Name = subgroup.Name;
                    tempSubgroup.Id = subgroup.Id;
                    tempSubgroup.AllowedDays = subgroup.AllowedDays;
                    tempSubgroup.Students = subgroup.Students;
                    tempSubgroup.Queue = subgroup.Queue;
                    subgroupsResult.Add(tempSubgroup);
                }
                return Result<IEnumerable<SubGroupDto>>.Success(subgroupsResult);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<SubGroupDto>>.Failure($"Error retrieving subgroups: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> UpdateStudents(SubGroupStudentsDto subGroupStudents)
        {
            try
            {
                SubGroup? subgroup = await _subGroupRepository.GetByIdAsync(subGroupStudents.SubGroupId);
                if (subgroup is null)
                {
                    throw new Exception("Subgroup not found");
                }

                await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);

                List<User> updatedStudents = new();
                foreach (string email in subGroupStudents.StudentsEmails)
                {
                    Result<User?> userResult = await _userService.GetUserByEmail(email);
                    if (userResult.IsSuccess && userResult.Data is not null)
                    {
                        updatedStudents.Add(userResult.Data);
                    }
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
                SubGroup? subgroup = await _subGroupRepository.GetByIdAsync(queuePlace.SubGroupId, sg => sg.Students);
                if (subgroup is null)
                {
                    throw new Exception("Subgroup not found");
                }
                // Get current user ID from token
                string currentUserId = _userService.GetCurrentUserId();
                // Check if user is part of the subgroup
                if (!subgroup.Students.Any(s => s.Id == currentUserId))
                {
                    // Optionally, if owner can add, check ownership
                    try
                    {
                        await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new UnauthorizedAccessException("User is not a member of this subgroup.");
                    }
                }
                var studentResult = await _userService.GetUserById(currentUserId);
                if (!studentResult.IsSuccess && studentResult.Data is not null) throw new Exception("User not found");
                QueuePlace newQueuePlace = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = currentUserId, // <-- Use current user ID
                    UserName = studentResult.Data.Name,
                    SubGroupId = queuePlace.SubGroupId,
                    TaskId = queuePlace.TaskId, // <-- SET TASK ID
                    SpecifiedTime = queuePlace.SpecifiedTime.ToUniversalTime() // Store as UTC
                };

                // Add to context (which adds to subgroup.Queue)
                await _queuePlaceRepository.AddAsync(newQueuePlace);
                await _unitOfWork.SaveChangesAsync();

                // Eager load the new place to return it
                QueuePlace? createdPlace =
                    await _queuePlaceRepository.GetByIdAsync(newQueuePlace.Id, q => q.Task, q => q.User);

                return
                    Result<SubGroup>.SuccessWithData(subgroup,
                        createdPlace); // Custom Result method if you need it, or just return subgroup
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
                QueuePlace? queuePlace = await _queuePlaceRepository.GetByIdAsync(queuePlaceId);
                if (queuePlace is null)
                {
                    throw new Exception("Queue place not found");
                }

                SubGroup? subgroup = await _subGroupRepository.GetByIdAsync(queuePlace.SubGroupId);
                if (subgroup is null)
                {
                    throw new Exception("Subgroup not found");
                }

                // Check if user is owner OR the user who made the booking
                string currentUserId = _userService.GetCurrentUserId();
                if (queuePlace.UserId != currentUserId)
                {
                    // If not the user, check if they are the owner
                    await EnsureCurrentUserIsOwnerOfCourse(subgroup.CourseId);
                }

                _queuePlaceRepository.Delete(queuePlace);
                await _unitOfWork.SaveChangesAsync();

                return Result<SubGroup>.Success(subgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"Error removing from queue: {ex.Message}");
            }
        }

        private async Task EnsureCurrentUserIsOwnerOfCourse(string courseId)
        {
            string currentUserId = _userService.GetCurrentUserId();
            Result<Course?> course = await _courseService.GetCourseById(courseId);
            if (!course.IsSuccess || course.Data is null)
            {
                throw new Exception("Course not found");
            }

            if (course.Data.OwnerId != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not the owner of this course");
            }
        }
    }
}
