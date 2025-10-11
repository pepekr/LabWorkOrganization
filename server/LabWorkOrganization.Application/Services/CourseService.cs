using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;

namespace LabWorkOrganization.Application.Services
{
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

        // TODO: ALL METHODS HERE MUST RETUR RESULT TYPE WITH SUCCESS FLAG AND ERROR MESSAGE IF NEEDED
        public async Task<Course> CreateCourse(Course course, bool useExternal)
        {
            try
            {
                if (useExternal)
                {
                    await _externalCrudRepository.AddAsync(course);
                }
                await _crudRepository.AddAsync(course);
                await _unitOfWork.SaveChangesAsync();
                return course;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred while creating the course: {ex.Message}");
                throw;
            }
        }
        public async Task<Course?> GetCourseById(Guid id, bool external)
        {
            try
            {
                if (external)
                {
                    return await _externalCrudRepository.GetByIdAsync(id);
                }
                return await _crudRepository.GetByIdAsync(id);
            }
            catch (Exception ex) { throw; }
        }
        public async Task<IEnumerable<Course>> GetAllCourses(bool isGetExternal)
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
                return togetherCourses;
            }
            catch (Exception ex) { throw; }
        }
        public async void UpdateCourse(Course course, bool updateExternal)
        {
            try
            {
                if (updateExternal)
                {
                    await _externalCrudRepository.UpdateAsync(course);
                }
                _crudRepository.Update(course);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex) { throw; }
        }

        public async void DeleteCourse(Guid id)
        {
            try
            {
                var course = await _crudRepository.GetByIdAsync(id);
                if (course != null)
                {
                    _crudRepository.Delete(course);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
