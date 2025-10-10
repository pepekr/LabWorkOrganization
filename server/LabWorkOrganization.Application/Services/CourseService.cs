using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;

namespace LabWorkOrganization.Application.Services
{
    public class CourseService
    {
        private readonly ICrudRepository<Course> _crudRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CourseService(ICrudRepository<Course> crudRepository, IUnitOfWork IUnitOfWork)
        {
            _crudRepository = crudRepository;
            _unitOfWork = IUnitOfWork;
        }
        public async Task<Course> CreateCourse(Course course)
        {
            await _crudRepository.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();
            return course;
        }
        public async Task<Course?> GetCourseById(Guid id)
        {
            return await _crudRepository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _crudRepository.GetAllAsync();
        }
        public async void UpdateCourse(Course course)
        {
            _crudRepository.Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async void DeleteCourse(Guid id)
        {
            var course = await _crudRepository.GetByIdAsync(id);
            if (course != null)
            {
                _crudRepository.Delete(course);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
