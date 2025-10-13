using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    // TODO: all service methods must accept dtos instead of entities
    public class CourseService
    {
        private readonly ICrudRepository<Course> _crudRepository;
        private readonly IExternalCrudRepo<Course> _externalCrudRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CourseService(ICrudRepository<Course> crudRepository, IUnitOfWork IUnitOfWork, IExternalCrudRepo<Course> IExternalCrudRepository)
        {
            _crudRepository = crudRepository;
            _unitOfWork = IUnitOfWork;
            _externalCrudRepository = IExternalCrudRepository;
        }
        public async Task<Result<Course>> CreateCourse(Course course, bool useExternal)
        {
            try
            {
                if (useExternal)
                {
                    await _externalCrudRepository.AddAsync(course);
                }
                await _crudRepository.AddAsync(course);
                await _unitOfWork.SaveChangesAsync();
                return Result<Course>.Success(course);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred while creating the course: {ex.Message}");
                return Result<Course>.Failure($"An error occurred while creating the course: {ex.Message}");
            }
        }
        public async Task<Result<Course?>> GetCourseById(Guid id, bool external)
        {
            try
            {
                if (external)
                {
                    return Result<Course?>.Success(await _externalCrudRepository.GetByIdAsync(id));
                }
                return Result<Course?>.Success(await _crudRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the course: {ex.Message}");
                return Result<Course?>.Failure($"An error occurred while creating the course: {ex.Message}");
            }
        }
        public async Task<Result<IEnumerable<Course>>> GetAllCourses(bool isGetExternal)
        {
            try
            {
                var courses = await _crudRepository.GetAllAsync();
                IEnumerable<Course> togetherCourses = courses ?? Enumerable.Empty<Course>();
                if (isGetExternal)
                {
                    togetherCourses = (courses ?? Enumerable.Empty<Course>())
                       .Concat(await _externalCrudRepository.GetAllAsync() ?? Enumerable.Empty<Course>());
                }
                return Result<IEnumerable<Course>>.Success(togetherCourses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the course: {ex.Message}");
                return Result<IEnumerable<Course>>.Failure($"An error occurred while creating the course: {ex.Message}");
            }
        }
        public async Task<Result<Course>> UpdateCourse(Course course, bool updateExternal)
        {
            try
            {
                if (updateExternal)
                {
                    if (course.ExternalId.HasValue)
                    {
                        await _externalCrudRepository.UpdateAsync(course, course.ExternalId.Value);
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
