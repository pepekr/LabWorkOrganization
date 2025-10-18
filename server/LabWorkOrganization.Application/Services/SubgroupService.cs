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
        private readonly ICourseScopedRepository<SubGroup> _crudRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        private readonly ICrudRepository<QueuePlace> _queuePlaceRepo;
        public SubgroupService(ICourseScopedRepository<SubGroup> crudRepository, IUnitOfWork IUnitOfWork, ICrudRepository<QueuePlace> IQueuePlaceRepo, IUserService userService, ICourseService course_service)
        {
            _crudRepository = crudRepository;
            _unitOfWork = IUnitOfWork;
            _queuePlaceRepo = IQueuePlaceRepo;
            _userService = userService;
            _courseService = course_service;
        }
        // IDEALLY MOVE THIS FUNC OUT TO USER SERVICE OR SOMETHING ELSE BUT DONT CARE FOR NOW
        private async Task IsCurrentUserOwnerOfCourse(string courseId)
        {
            var currentUserId = _userService.GetCurrentUserId();
            var course = await _courseService.GetCourseById(courseId);
            if (!course.IsSuccess)
            {
                throw new ArgumentException(course.ErrorMessage);
            }
            if (course.Data?.OwnerId.ToString() != currentUserId)
            {
                throw new UnauthorizedAccessException("User not authorized to perform this action");
            };
        }
        // MAYBE IMPLEMENT LOGIC WHERE ONLY OWNER OF COURSE CAN CREATE SUBGROUPS
        public async Task<Result<SubGroup>> CreateSubgroup(SubGroupCreationalDto subgroup)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(subgroup);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                var courseResult = await _courseService.GetCourseById(subgroup.CourseId);
                if (!courseResult.IsSuccess) // or current user isnt an owner return
                {
                    throw new ArgumentException(courseResult.ErrorMessage);
                }
                var sugGroupStudents = new List<User?>();
                foreach (var stEmail in subgroup.StudentsEmails)
                {
                    var result = await _userService.GetUserByEmail(stEmail);
                    if (result.IsSuccess) sugGroupStudents.Add(result.Data);
                }
                var newSubgroup = new SubGroup
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = subgroup.Name,
                    CourseId = subgroup.CourseId,
                    AllowedDays = subgroup.AllowedDays,
                    Students = sugGroupStudents.Where(s => s is not null).Select(s => s!).ToList()
                };

                await _crudRepository.AddAsync(newSubgroup);
                await _unitOfWork.SaveChangesAsync();
                return Result<SubGroup>.Success(newSubgroup);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred while creating the subgroup: {ex.Message}");
                return Result<SubGroup>.Failure($"An error occurred while creating the subgroup: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> DeleteSubgroup(SubGroup subgroup)
        {
            try
            {
                var currentUserId = _userService.GetCurrentUserId();
                if (currentUserId is null) throw new Exception("User not authorized");
                var course = await _courseService.GetCourseById(subgroup.CourseId);
                if (!course.IsSuccess || course.Data is null) throw new Exception("Course not found");
                if (course.Data.OwnerId.ToString() != currentUserId) throw new Exception("User not authorized to delete this subgroup");
                _crudRepository.Delete(subgroup);
                await _unitOfWork.SaveChangesAsync();
                return Result<SubGroup>.Success(subgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"An error occurred while deleting the subgroup: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> GetAllByCourseId(string courseId)
        {
            try
            {
                var subgroups = await _crudRepository.GetAllByCourseIdAsync(courseId) ?? new List<SubGroup>();
                return Result<SubGroup>.Success(subgroups.FirstOrDefault()!);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"An error occurred while retrieving subgroups: {ex.Message}");
            }
        }

        public async Task<Result<SubGroup>> UpdateStudents(SubGroupStudentsDto subGroupStudents)
        {
            try
            {
                var subgroup = await _crudRepository.GetByIdAsync(subGroupStudents.SubGroupId);
                if (subgroup is null) throw new Exception("Subgroup not found");
                var currentUserId = _userService.GetCurrentUserId();
                if (currentUserId is null) throw new Exception("User not authorized");
                var course = await _courseService.GetCourseById(subgroup.CourseId);
                if (!course.IsSuccess || course.Data is null) throw new Exception("Course not found");
                if (course.Data.OwnerId.ToString() != currentUserId) throw new Exception("User not authorized to update this subgroup");
                var newStudents = new List<User?>();
                foreach (var stEmail in subGroupStudents.StudentsEmails)
                {
                    var result = await _userService.GetUserByEmail(stEmail);
                    if (result.IsSuccess) newStudents.Add(result.Data);
                }
                subgroup.Students = newStudents.Where(s => s is not null).Select(s => s!).ToList();
                _crudRepository.Update(subgroup);
                await _unitOfWork.SaveChangesAsync();
                return Result<SubGroup>.Success(subgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"An error occurred while updating the subgroup: {ex.Message}");
            }
        }


        /// REFACTOR !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! might be for adding not updating(overwritting) 
        public async Task<Result<SubGroup>> AddToQueue(QueuePlaceCreationalDto queuePlace)
        {
            try
            {
                var subgroup = await _crudRepository.GetByIdAsync(queuePlace.SubGroupId);
                if (subgroup is null) throw new Exception("Subgroup not found");
                var course = await _courseService.GetCourseById(subgroup.CourseId);
                if (!course.IsSuccess || course.Data is null) throw new Exception("Course not found");
                await IsCurrentUserOwnerOfCourse(course.Data.Id);
                if (queuePlace.UserId.ToString() != _userService.GetCurrentUserId()) throw new Exception("User not authorized to add to queue in this subgroup");

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
                return Result<SubGroup>.Failure($"An error occurred while adding to queue: {ex.Message}");
            }
        }
        public async Task<Result<SubGroup>> RemoveFromQueue(string queuePlaceId)
        {
            try
            {
                var queuePlace = await _queuePlaceRepo.GetByIdAsync(queuePlaceId);
                if (queuePlace is null) throw new Exception("Queue place not found");
                var subgroup = await _crudRepository.GetByIdAsync(queuePlace.SubGroupId);
                if (subgroup is null) throw new Exception("Subgroup not found");
                var course = await _courseService.GetCourseById(subgroup.CourseId);
                if (!course.IsSuccess || course.Data is null) throw new Exception("Course not found");
                await IsCurrentUserOwnerOfCourse(course.Data.Id);
                if (queuePlace.UserId.ToString() != _userService.GetCurrentUserId()) throw new Exception("User not authorized to remove from queue in this subgroup");

                subgroup.Queue.Remove(queuePlace);
                _queuePlaceRepo.Delete(queuePlace);
                await _unitOfWork.SaveChangesAsync();
                return Result<SubGroup>.Success(subgroup);
            }
            catch (Exception ex)
            {
                return Result<SubGroup>.Failure($"An error occurred while removing from queue: {ex.Message}");
            }
        }
    }
}
