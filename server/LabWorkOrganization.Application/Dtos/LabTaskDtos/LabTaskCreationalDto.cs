using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos.LabTaskDtos
{
    public class LabTaskCreationalDto
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public bool IsSentRequired { get; set; }
        [Required]
        public TimeSpan TimeLimitPerStudent { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        public bool UseExternal { get; set; }

    }

}
