using LabWorkOrganization.Application.Dtos.CourseDtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface ICourseService
    {
        Task<Result<Course>> CreateCourse(CourseCreationalDto course, bool useExternal);
        Task<Result<Course>> DeleteCourse(string id);
        Task<Result<IEnumerable<Course>>> GetAllCourses(bool isGetExternal = false);
        Task<Result<Course?>> GetCourseById(string id, bool external = false);
        Task<Result<IEnumerable<Course>>> GetAllCoursesByUserAsync(bool isGetExternal = false);
        Task<Result<Course>> UpdateCourse(Course course, bool updateExternal = false);
    }
}
