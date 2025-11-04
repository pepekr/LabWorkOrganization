using LabWorkOrganization.Application.Dtos.CourseDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Validation;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUserScopedRepository<Course> _crudRepository;

        //private readonly IExternalCrudRepo<Course> _externalCrudRepository;
        private readonly IExternalCrudRepoFactory _externalCrudFactory;
        private readonly IExternalTokenService _externalTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public CourseService(IUserScopedRepository<Course> crudRepository, IUnitOfWork IUnitOfWork,
            IExternalCrudRepoFactory IExternalCrudFactory, IUserService userService,
            IExternalTokenService IExternalTokenService)
        {
            _crudRepository = crudRepository;
            _unitOfWork = IUnitOfWork;
            _externalCrudFactory = IExternalCrudFactory;
            _userService = userService;
            _externalTokenService = IExternalTokenService;
        }

        public async Task<Result<Course>> CreateCourse(CourseCreationalDto course, bool useExternal)
        {
            try
            {
                List<string> errors = ValidationHelper.Validate(course);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }

                string userId = _userService.GetCurrentUserId();
                // CHANGE THIS TO A JUST GET SUB ID FROM TOKEN
                Result<User?> user = await _userService.GetUserById(userId);
                if (!user.IsSuccess)
                {
                    throw new ArgumentException("User not found");
                }


                Course newCourse = new()
                {
                    // NOT ENTERING EXTERNAL ID EXTERNAL API WILL HANDLE IT
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = userId,
                    OwnerExternalId = user.Data.SubGoogleId,
                    Name = course.Name,
                    LessonDuration = course.LessonDuration,
                    EndOfCourse = course.EndOfCourse
                };
                if (useExternal)
                {
                    if (user.Data.SubGoogleId is null)
                    {
                        throw new ArgumentException("User does not have an external ID");
                    }

                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    IExternalCrudRepo<Course> repo =
                        _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    Course check = await repo.AddAsync(newCourse);
                    if (check.ExternalId is not null)
                    {
                        newCourse.ExternalId = check.ExternalId;
                    }
                }

                await _crudRepository.AddAsync(newCourse);
                await _unitOfWork.SaveChangesAsync();
                return Result<Course>.Success(newCourse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the course: {ex.Message}");
                return Result<Course>.Failure($"An error occurred while creating the course: {ex.Message}");
            }
        }

        public async Task<Result<Course?>> GetCourseById(string id, bool external = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }

                Course? localCourse = await _crudRepository.GetByIdAsync(id);

                // Якщо вимагаємо external
                if (external)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    IExternalCrudRepo<Course> repo =
                        _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    Course? externalCourse = await repo.GetByIdAsync(id);

                    if (externalCourse == null)
                    {
                        throw new Exception("External course not found");
                    }

                    // Якщо локального курсу немає — створюємо копію
                    if (localCourse == null)
                    {
                        Course newLocal = new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            ExternalId = externalCourse.ExternalId ?? externalCourse.Id,
                            OwnerExternalId = externalCourse.OwnerExternalId,
                            Name = externalCourse.Name,
                            OwnerId = userId,
                            EndOfCourse = DateTime.UtcNow.AddMonths(3),
                            LessonDuration = TimeSpan.FromHours(1)
                        };

                        await _crudRepository.AddAsync(newLocal);
                        await _unitOfWork.SaveChangesAsync();
                        localCourse = newLocal;
                    }

                    if (localCourse.OwnerId != userId)
                    {
                        return Result<Course?>.Failure("Access denied to the course");
                    }

                    return Result<Course?>.Success(localCourse);
                }

                if (localCourse == null)
                {
                    return Result<Course?>.Failure("Course not found");
                }

                return Result<Course?>.Success(localCourse);
            }
            catch (Exception ex)
            {
                return Result<Course?>.Failure($"An error occurred while getting the course: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Course>>> GetAllCourses(bool isGetExternal = false)
        {
            try
            {
                IEnumerable<Course>? courses = await _crudRepository.GetAllAsync();
                IEnumerable<Course> togetherCourses = courses ?? Enumerable.Empty<Course>();
                if (isGetExternal)
                {
                    string userId = _userService.GetCurrentUserId();
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }

                    IExternalCrudRepo<Course> repo =
                        _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    togetherCourses = (courses ?? Enumerable.Empty<Course>())
                        .Concat(await repo.GetAllAsync() ?? Enumerable.Empty<Course>());
                }

                return Result<IEnumerable<Course>>.Success(togetherCourses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the course: {ex.Message}");
                return Result<IEnumerable<Course>>.Failure(
                    $"An error occurred while creating the course: {ex.Message}");
            }
        }


        //**********************FIX THIS <3 ***********************************************************
        public async Task<Result<IEnumerable<Course>>> GetAllCoursesByUserAsync(bool includeExternal = false)
        {
            try
            {

                // inject subgroup repo, instead of subgroup service, get all courses from subgroups that user in. 
                // if u cant find method check out Scoped repos implementation

                string userId = _userService.GetCurrentUserId();
                IEnumerable<Course> internalCourses = await _crudRepository.GetAllByUserIdAsync(userId)
                                                      ?? Enumerable.Empty<Course>();

                IEnumerable<Course> userCourses = internalCourses;

                if (includeExternal)
                {
                    Result<string> accessTokenResult =
                        await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                    {
                        throw new Exception(accessTokenResult.ErrorMessage);
                    }


                    IExternalCrudRepo<Course> repo =
                        _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    IEnumerable<Course> externalCourses = await repo.GetAllAsync() ?? Enumerable.Empty<Course>();


                    userCourses = internalCourses.Concat(externalCourses);
                }

                return Result<IEnumerable<Course>>.Success(userCourses);
            }
            catch (Exception ex)
            {
                string message = $"An error occurred while retrieving user courses: {ex.Message}";
                Console.WriteLine(message);
                return Result<IEnumerable<Course>>.Failure(message);
            }
        }

        public async Task<Result<Course>> UpdateCourse(CourseUpdateDto course, bool updateExternal = false)
        {
            try
            {
                List<string> errors = ValidationHelper.Validate(course);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }

                string userId = _userService.GetCurrentUserId();
                Course? courseDb = await _crudRepository.GetByIdAsync(course.Id);
                if (courseDb == null)
                {
                    return Result<Course>.Failure("Course not found");
                }

                if (courseDb.OwnerId != userId)
                {
                    return Result<Course>.Failure("Access denied to the course");
                }

                courseDb.Name = course.Name;
                courseDb.LessonDuration = course.LessonDuration;
                courseDb.EndOfCourse = course.EndOfCourse;
                if (updateExternal)
                {
                    if (courseDb.ExternalId is not null)
                    {
                        Result<string> accessTokenResult =
                            await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                        if (!accessTokenResult.IsSuccess)
                        {
                            throw new Exception(accessTokenResult.ErrorMessage);
                        }

                        IExternalCrudRepo<Course> repo =
                            _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                        await repo.UpdateAsync(courseDb, courseDb.ExternalId);
                    }
                }

                _crudRepository.Update(courseDb);
                await _unitOfWork.SaveChangesAsync();
                return Result<Course>.Success(courseDb);
            }
            catch (Exception ex)
            {
                return Result<Course>.Failure($"An error occurred while updating the course: {ex.Message}");
            }
        }

        public async Task<Result<Course>> DeleteCourse(string id)
        {
            try
            {
                string userId = _userService.GetCurrentUserId();
                Course? course = await _crudRepository.GetByIdAsync(id);
                if (course.OwnerId != userId)
                {
                    return Result<Course>.Failure("Access denied to the course");
                }

                if (course != null)
                {
                    _crudRepository.Delete(course);
                    await _unitOfWork.SaveChangesAsync();
                    return Result<Course>.Success(course);
                }

                return Result<Course>.Failure("Course not found");
            }
            catch (Exception ex)
            {
                return Result<Course>.Failure($"An error occurred while updating the course: {ex.Message}");
            }
        }
    }
}
