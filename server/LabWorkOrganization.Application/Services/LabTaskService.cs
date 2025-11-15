using LabWorkOrganization.Application.Dtos.LabTaskDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Validation;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class LabTaskService : ILabTaskService
    {
        private readonly ICourseService _courseService;
        private readonly ICourseScopedRepository<LabTask> _crudRepository;
        private readonly IExternalCrudRepoFactory _externalCrudFactory;
        private readonly IExternalTokenService _externalTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public LabTaskService(IUnitOfWork IUnitOfWork, IExternalCrudRepoFactory IExternalCrudFactory,
            ICourseScopedRepository<LabTask> taskRepo, ICourseService courseService, IUserService userService,
            IExternalTokenService IExternalTokenService)
        {
            _unitOfWork = IUnitOfWork;
            _externalCrudFactory = IExternalCrudFactory;
            _crudRepository = taskRepo;
            _courseService = courseService;
            _userService = userService;
            _externalTokenService = IExternalTokenService;
        }

        public async Task<Result<LabTask>> CreateTask(LabTaskCreationalDto labTask, bool useExternal)
        {
            try
            {
                await IsCurrentUserOwnerOfCourse(labTask.CourseId);
                List<string> errors = ValidationHelper.Validate(labTask);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }

                LabTask newTask = new()
                {
                    // NOT ENTERING EXTERNAL ID EXTERNAL API WILL HANDLE IT
                    Id = Guid.NewGuid().ToString(),
                    Title = labTask.Title,
                    DueDate = labTask.DueDate,
                    Description = labTask.Description,
                    IsSentRequired = labTask.IsSentRequired,
                    TimeLimitPerStudent = labTask.TimeLimitPerStudent,
                    CourseId = labTask.CourseId
                };
                if (useExternal)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    Result<Course?>? localCourse = await _courseService.GetCourseById(labTask.CourseId);
                    if (localCourse is null || !localCourse.IsSuccess)
                    {
                        throw new Exception("Course id is not valid");
                    }

                    if (localCourse.Data.ExternalId is null)
                    {
                        throw new Exception("Course does not have third party copy");
                    }

                    IExternalCrudRepo<LabTask> repo = _externalCrudFactory.Create<LabTask>(
                        $"https://classroom.googleapis.com/v1/courses/{localCourse.Data.ExternalId}/courseWork");
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

        public async Task<Result<LabTask?>> GetTaskById(string id, string courseId, bool external = false)
        {
            try
            {
                if (external)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    IExternalCrudRepo<LabTask> repo =
                        _externalCrudFactory.Create<LabTask>(
                            $"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    return Result<LabTask?>.Success(await repo.GetByIdAsync(id));
                }

                return Result<LabTask?>.Success(await _crudRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return Result<LabTask?>.Failure($"An error occurred while getting the task: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<LabTask>>> GetAllTasksByCourseId(string courseId, bool external = false)
        {
            try
            {
                IEnumerable<LabTask> localTasks =
                    await _crudRepository.GetAllByCourseIdAsync(courseId) ?? new List<LabTask>();

                if (external)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    ICourseScopedExternalRepository<LabTask> repo = (ICourseScopedExternalRepository<LabTask>)
                        _externalCrudFactory.Create<LabTask>(
                            $"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    IEnumerable<LabTask> externalTasks =
                        await repo.GetAllByCourseIdAsync(courseId) ?? Enumerable.Empty<LabTask>();

                    // якщо локально пусто — створюємо локальні копії
                    if (!localTasks.Any() && externalTasks.Any())
                    {
                        foreach (LabTask t in externalTasks)
                        {
                            LabTask localCopy = new()
                            {
                                Id = Guid.NewGuid().ToString(),
                                ExternalId = t.ExternalId ?? t.Id,
                                Title = t.Title,
                                DueDate = t.DueDate,
                                CourseId = courseId,
                                IsSentRequired = false,
                                TimeLimitPerStudent = TimeSpan.FromMinutes(30)
                            };
                            await _crudRepository.AddAsync(localCopy);
                        }

                        await _unitOfWork.SaveChangesAsync();
                        localTasks = await _crudRepository.GetAllByCourseIdAsync(courseId);
                    }

                    return Result<IEnumerable<LabTask>>.Success(localTasks);
                }

                return Result<IEnumerable<LabTask>>.Success(localTasks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<LabTask>>.Failure($"Error getting tasks: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<LabTask>>> GetAllTasks(string courseId, bool isGetExternal = false)
        {
            try
            {
                IEnumerable<LabTask> tasks = await _crudRepository.GetAllAsync();
                if (isGetExternal)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    IExternalCrudRepo<LabTask> repo =
                        _externalCrudFactory.Create<LabTask>(
                            $"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    tasks.Concat(await repo.GetAllAsync() ?? Enumerable.Empty<LabTask>());
                }

                return Result<IEnumerable<LabTask>>.Success(tasks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<LabTask>>.Failure($"An error occured while getting the task: ${ex.Message}");
            }
        }

        public async Task<Result<LabTask>> UpdateTask(string id, LabTaskCreationalDto labTask,
            bool updateExternal = false)
        {
            try
            {
                await IsCurrentUserOwnerOfCourse(labTask.CourseId);
                List<string> errors = ValidationHelper.Validate(labTask);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }

                LabTask? existingLabTask = GetTaskById(id, labTask.CourseId, updateExternal).Result.Data;

                existingLabTask.Title = labTask.Title;
                existingLabTask.DueDate = labTask.DueDate;
                existingLabTask.Description = labTask.Description;
                existingLabTask.IsSentRequired = labTask.IsSentRequired;
                existingLabTask.TimeLimitPerStudent = labTask.TimeLimitPerStudent;

                if (updateExternal && existingLabTask.ExternalId is not null)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    IExternalCrudRepo<LabTask> repo =
                        _externalCrudFactory.Create<LabTask>(
                            $"https://classroom.googleapis.com/v1/courses/{labTask.CourseId}/courseWork");


                    await repo.UpdateAsync(existingLabTask, existingLabTask.ExternalId);
                }

                _crudRepository.Update(existingLabTask);
                await _unitOfWork.SaveChangesAsync();
                return Result<LabTask>.Success(existingLabTask);
            }
            catch (Exception ex)
            {
                return Result<LabTask>.Failure($"An error occured while updating the task: ${ex.Message}");
            }
        }

        public async Task<Result<LabTask>> DeleteTask(string id, string courseId, bool deleteExternal)
        {
            try
            {
                LabTask? task = await _crudRepository.GetByIdAsync(id);
                if (task is null)
                {
                    throw new Exception("Task not found");
                }

                await IsCurrentUserOwnerOfCourse(task.CourseId);
                if (deleteExternal && task.ExternalId is not null)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    IExternalCrudRepo<LabTask> repo =
                        _externalCrudFactory.Create<LabTask>(
                            $"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    await repo.DeleteAsync(task.ExternalId);
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
        
        // new method 4 task searching
        public async Task<Result<IEnumerable<LabTask>>> SearchTask(string courseId, string? title, DateTime? dueDate,
            bool useExternal)
        {
            try
            {
                string titleStart = string.Empty;
                string titleEnd = string.Empty;
                if (!string.IsNullOrEmpty(title))
                {
                    string[] parts = title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                        (titleStart, titleEnd) = (parts[0], parts[1]);
                    else
                        (titleStart, titleEnd) = (parts[0], string.Empty);
                }
                if (useExternal)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }
                    IExternalCrudRepo<LabTask> repo =
                        _externalCrudFactory.Create<LabTask>(
                            $"https://classroom.googleapis.com/v1/courses/{courseId}/courseWork");

                    var externalTasks = await repo.GetAllAsync();
                    var filteredExternalTasks = externalTasks
                        .Where(t =>
                            (string.IsNullOrEmpty(title) || 
                             (t.Title.ToLowerInvariant().StartsWith(titleStart.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase)
                              && t.Title.ToLowerInvariant().EndsWith(titleEnd.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))) &&
                            (!dueDate.HasValue || t.DueDate >= dueDate))
                        .ToList();

                    return Result<IEnumerable<LabTask>>.Success(filteredExternalTasks);
                }

                var tasks = await _crudRepository.GetAllAsync();
                var filteredTasks = tasks
                    .Where(t => // пошук за початком та кінцем назви та за датою
                        (string.IsNullOrEmpty(title) || 
                         (t.Title.ToLowerInvariant().StartsWith(titleStart.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase)
                          && t.Title.ToLowerInvariant().EndsWith(titleEnd.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))) &&
                        (!dueDate.HasValue || t.DueDate >= dueDate.Value))
                    .ToList();

                return Result<IEnumerable<LabTask>>.Success(filteredTasks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<LabTask>>.Failure($"An error occurred while searching tasks: {ex.Message}");
            }
        }

        private async Task IsCurrentUserOwnerOfCourse(string courseId)
        {
            string currentUserId = _userService.GetCurrentUserId();
            Result<Course?> course = await _courseService.GetCourseById(courseId);
            if (!course.IsSuccess)
            {
                throw new ArgumentException(course.ErrorMessage);
            }

            if (course.Data?.OwnerId != currentUserId)
            {
                throw new UnauthorizedAccessException("User not authorized to perform this action");
            }
            
        }
    }
}
