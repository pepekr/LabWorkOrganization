using LabWorkOrganization.Domain.Intefaces;

namespace LabWorkOrganization.Application.DTOs.Course
{
    public class CourseCreationalDto : ICreationalDto
    {
        public string Name { get; set; } = null!;
        public TimeSpan LessonDuration { get; set; }
        public DateTime EndOfCourse { get; set; }
    }
}
