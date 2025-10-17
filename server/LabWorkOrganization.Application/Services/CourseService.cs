using LabWorkOrganization.Application.Dtos.CourseDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICrudRepository<Course> _crudRepository;
        //private readonly IExternalCrudRepo<Course> _externalCrudRepository;
        private readonly IExternalCrudRepoFactory _externalCrudFactory;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        public CourseService(ICrudRepository<Course> crudRepository, IUnitOfWork IUnitOfWork, IExternalCrudRepoFactory IExternalCrudFactory, IUserService userService)
        {
            _crudRepository = crudRepository;
            _unitOfWork = IUnitOfWork;
            _externalCrudFactory = IExternalCrudFactory;
            _userService = userService;
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
                var userId = Guid.Parse(_userService.GetCurrentUserId());

                var newCourse = new Course
                { // NOT ENTERING EXTERNAL ID EXTERNAL API WILL HANDLE IT
                    Id = Guid.NewGuid(),
                    OwnerId = userId, // NEED TO IMPLEMENT AUTHORIZATION
                    Name = course.Name,
                    LessonDuration = course.LessonDuration,
                    EndOfCourse = course.EndOfCourse
                };
                if (useExternal)
                {
                    var repo = _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                    await repo.AddAsync(newCourse);
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
        public async Task<Result<Course?>> GetCourseById(Guid id, bool external = false)
        {
            try
            {
                if (id.Equals(null)) throw new ArgumentNullException(nameof(id));
                if (external)
                {
                    var repo = _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");

                    return Result<Course?>.Success(await repo.GetByIdAsync(id));
                }
                return Result<Course?>.Success(await _crudRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the course: {ex.Message}");
                return Result<Course?>.Failure($"An error occurred while creating the course: {ex.Message}");
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
        public async Task<Result<Course>> UpdateCourse(Course course, bool updateExternal = false)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(course);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }

                if (updateExternal)
                {
                    if (course.ExternalId.HasValue)
                    {
                        var repo = _externalCrudFactory.Create<Course>("https://classroom.googleapis.com/v1/courses");
                        await repo.UpdateAsync(course, course.ExternalId.Value);
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

        public async Task<Result<Course>> DeleteCourse(Guid id)
        {
            try
            {
                var course = await _crudRepository.GetByIdAsync(id);
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
