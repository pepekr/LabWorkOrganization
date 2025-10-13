using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos
{
    public class CourseCreationalDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public TimeSpan LessonDuration { get; set; }
        [Required]
        public DateTime EndOfCourse { get; set; }
    }

}
