using LabWorkOrganization.Application.Dtos.CourseDtos;
using LabWorkOrganization.Application.Interfaces;
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
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExternalTokenService _externalTokenService;
        public CourseService(IUserScopedRepository<Course> crudRepository, IUnitOfWork IUnitOfWork, IExternalCrudRepoFactory IExternalCrudFactory, IUserService userService, IExternalTokenService IExternalTokenService)
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
                var errors = Validation.ValidationHelper.Validate(course);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                var userId = _userService.GetCurrentUserId();
                // CHANGE THIS TO A JUST GET SUB ID FROM TOKEN
                var user = await _userService.GetUserById(userId);
                if (!user.IsSuccess)
                {
                    throw new ArgumentException("User not found");
                }
                if (user.Data.SubGoogleId is null)
                {
                    throw new ArgumentException("User does not have an external ID");
                }

                var newCourse = new Course
                { // NOT ENTERING EXTERNAL ID EXTERNAL API WILL HANDLE IT
                    Id = Guid.NewGuid().ToString(),
                    OwnerId = userId,
                    OwnerExternalId = user.Data.SubGoogleId,
                    Name = course.Name,
                    LessonDuration = course.LessonDuration,
                    EndOfCourse = course.EndOfCourse
                };
                if (useExternal)
                {
                    var accessTokenResult = await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                        throw new Exception(accessTokenResult.ErrorMessage);
                    var repo = _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    var check = await repo.AddAsync(newCourse);
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
                    throw new ArgumentNullException(nameof(id));

                var localCourse = await _crudRepository.GetByIdAsync(id);

                // Якщо вимагаємо external
                if (external)
                {
                    var userId = _userService.GetCurrentUserId();
                    var accessTokenResult = await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                        throw new Exception(accessTokenResult.ErrorMessage);

                    var repo = _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    var externalCourse = await repo.GetByIdAsync(id);

                    if (externalCourse == null)
                        throw new Exception("External course not found");

                    // Якщо локального курсу немає — створюємо копію
                    if (localCourse == null)
                    {
                        var newLocal = new Course
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
                    return Result<Course?>.Failure("Course not found");

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
                var courses = await _crudRepository.GetAllAsync();
                IEnumerable<Course> togetherCourses = courses ?? Enumerable.Empty<Course>();
                if (isGetExternal)
                {
                    var userId = _userService.GetCurrentUserId();
                    var accessTokenResult = await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                        throw new Exception(accessTokenResult.ErrorMessage);
                    var repo = _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    togetherCourses = (courses ?? Enumerable.Empty<Course>())
                       .Concat(await repo.GetAllAsync() ?? Enumerable.Empty<Course>());
                }
                return Result<IEnumerable<Course>>.Success(togetherCourses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the course: {ex.Message}");
                return Result<IEnumerable<Course>>.Failure($"An error occurred while creating the course: {ex.Message}");
            }
        }
        public async Task<Result<IEnumerable<Course>>> GetAllCoursesByUserAsync(bool includeExternal = false)
        {
            try
            {
                var userId = _userService.GetCurrentUserId();
                var internalCourses = await _crudRepository.GetAllByUserIdAsync(userId)
                                       ?? Enumerable.Empty<Course>();

                IEnumerable<Course> userCourses = internalCourses;

                if (includeExternal)
                {
                    var accessTokenResult = await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                    if (!accessTokenResult.IsSuccess)
                        throw new Exception(accessTokenResult.ErrorMessage);


                    var repo = _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    var externalCourses = await repo.GetAllAsync() ?? Enumerable.Empty<Course>();


                    userCourses = internalCourses.Concat(externalCourses);
                }

                return Result<IEnumerable<Course>>.Success(userCourses);
            }
            catch (Exception ex)
            {
                var message = $"An error occurred while retrieving user courses: {ex.Message}";
                Console.WriteLine(message);
                return Result<IEnumerable<Course>>.Failure(message);
            }
        }

        public async Task<Result<Course>> UpdateCourse(Course course, bool updateExternal = false)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(course);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                var userId = _userService.GetCurrentUserId();
                var courseDb = await _crudRepository.GetByIdAsync(course.Id);
                if (courseDb == null)
                {
                    return Result<Course>.Failure("Course not found");
                }
                if (courseDb.OwnerId != userId)
                {
                    return Result<Course>.Failure("Access denied to the course");
                }
                if (updateExternal)
                {
                    if (course.ExternalId is not null)
                    {

                        var accessTokenResult = await _externalTokenService.GetAccessTokenFromDbAsync(userId, "Google");
                        if (!accessTokenResult.IsSuccess)
                            throw new Exception(accessTokenResult.ErrorMessage);
                        var repo = _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                        await repo.UpdateAsync(course, course.ExternalId);
                    }
                }

                _crudRepository.Update(course);
                await _unitOfWork.SaveChangesAsync();
                return Result<Course>.Success(course);
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
                var userId = _userService.GetCurrentUserId();
                var course = await _crudRepository.GetByIdAsync(id);
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
