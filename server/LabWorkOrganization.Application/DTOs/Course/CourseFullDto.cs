using LabWorkOrganization.Domain.Intefaces;

namespace LabWorkOrganization.Application.DTOs.Course
{
    public class CourseFullDto : IFullDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public TimeSpan LessonDuration { get; set; }
        public DateTime EndOfCourse { get; set; }
    }
}
