using LabWorkOrganization.Application.Dtos.LabTaskDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class LabTaskService : ILabTaskService
    {
        private readonly ICourseScopedRepository<LabTask> _crudRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExternalCrudRepoFactory _externalCrudFactory;
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        public LabTaskService(IUnitOfWork IUnitOfWork, IExternalCrudRepoFactory IExternalCrudFactory, ICourseScopedRepository<LabTask> taskRepo, ICourseService courseService, IUserService userService)
        {
            _unitOfWork = IUnitOfWork;
            _externalCrudFactory = IExternalCrudFactory;
            _crudRepository = taskRepo;
            _courseService = courseService;
            _userService = userService;
        }
        private async Task IsCurrentUserOwnerOfCourse(Guid courseId)
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
        public async Task<Result<LabTask>> CreateTask(LabTaskCreationalDto labTask, bool useExternal)
        {
            try
            {
                await IsCurrentUserOwnerOfCourse(labTask.CourseId);
                var errors = Validation.ValidationHelper.Validate(labTask);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                var newTask = new LabTask
                { // NOT ENTERING EXTERNAL ID EXTERNAL API WILL HANDLE IT
                    Id = Guid.NewGuid(),
                    Title = labTask.Title,
                    DueDate = labTask.DueDate,
                    IsSentRequired = labTask.IsSentRequired,
                    TimeLimitPerStudent = labTask.TimeLimitPerStudent,
                    CourseId = labTask.CourseId
                };
                if (useExternal)
                {
                    var repo = _externalCrudFactory.Create<LabTask>($"https://classroom.googleapis.com/v1/courses/{labTask.CourseId}/courseWork");
                    await repo.AddAsync(newTask);
                }
                await _crudRepository.AddAsync(newTask);
                await _unitOfWork.SaveChangesAsync();
                return Result<LabTask>.Success(newTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the task: {ex.Message}");
                return Result<LabTask>.Failure($"An error occurred while creating the task: {ex.Message}");
            }
        }
        public async Task<Result<LabTask?>> GetTaskById(Guid id, Guid courseId, bool external = false)
        {
            try
            {
                if (external)
                {
                    var repo = _externalCrudFactory.Create<LabTask>($"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    return Result<LabTask?>.Success(await repo.GetByIdAsync(id));
                }
                return Result<LabTask?>.Success(await _crudRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return Result<LabTask?>.Failure($"An error occurred while getting the task: {ex.Message}");
            }

        }

        public async Task<Result<IEnumerable<LabTask>>> GetAllTasksByCourseId(Guid courseId, bool external = false)
        {
            try
            {
                if (external)
                {
                    var repo = (ICourseScopedExternalRepository<LabTask>)_externalCrudFactory.Create<LabTask>($"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    return Result<IEnumerable<LabTask>>.Success(await repo.GetAllByCourseIdAsync(courseId));
                }
                return Result<IEnumerable<LabTask>>.Success(await _crudRepository.GetAllByCourseIdAsync(courseId));
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<LabTask>>.Failure($"An error occurred while getting the task: {ex.Message}");
            }

        }
        public async Task<Result<IEnumerable<LabTask>>> GetAllTasks(Guid courseId, bool isGetExternal = false)
        {
            try
            {
                var tasks = await _crudRepository.GetAllAsync();
                if (isGetExternal)
                {
                    var repo = _externalCrudFactory.Create<LabTask>($"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    tasks.Concat(await repo.GetAllAsync() ?? Enumerable.Empty<LabTask>());
                }
                return Result<IEnumerable<LabTask>>.Success(tasks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<LabTask>>.Failure($"An error occured while getting the task: ${ex.Message}");
            }
        }
        public async Task<Result<LabTask>> UpdateTask(LabTask labTask, bool updateExternal = false)
        {
            try
            {
                await IsCurrentUserOwnerOfCourse(labTask.CourseId);
                var errors = Validation.ValidationHelper.Validate(labTask);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                if (updateExternal && labTask.ExternalId is not null)
                {
                    var repo = _externalCrudFactory.Create<LabTask>($"https://classroom.googleapis.com/v1/courses/{labTask.CourseId}/courseWork");

                    await repo.UpdateAsync(labTask, labTask.ExternalId.Value);
                }
                _crudRepository.Update(labTask);
                await _unitOfWork.SaveChangesAsync();
                return Result<LabTask>.Success(labTask);
            }
            catch (Exception ex)
            {
                return Result<LabTask>.Failure($"An error occured while updating the task: ${ex.Message}");
            }
        }

        public async Task<Result<LabTask>> DeleteTask(Guid id, Guid courseId, bool deleteExternal)
        {
            try
            {
                var task = await _crudRepository.GetByIdAsync(id);
                if (task is null)
                {
                    throw new Exception("Task not found");
                }
                await IsCurrentUserOwnerOfCourse(task.CourseId);
                if (deleteExternal && task.ExternalId is not null)
                {
                    var repo = _externalCrudFactory.Create<LabTask>($"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    await repo.DeleteAsync(task.ExternalId.Value);
                }
                _crudRepository.Delete(task);
                await _unitOfWork.SaveChangesAsync();
                return Result<LabTask>.Success(task);
            }
            catch (Exception ex)
            {
                return Result<LabTask>.Failure($"An error occured while deleting the task: ${ex.Message}");
            }

        }
    }
}
