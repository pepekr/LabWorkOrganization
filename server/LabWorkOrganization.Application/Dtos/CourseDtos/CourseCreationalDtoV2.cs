using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos.CourseDtos
{
    public class CourseCreationalDtoV2
    {
        [Required] public string Name { get; set; } = null!;

        public string Description { get; set; } = "-";
        [Required] public TimeSpan LessonDuration { get; set; }

        [Required] public DateTime EndOfCourse { get; set; }

        public bool UseExternal { get; set; }
    }
}
