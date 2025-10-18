using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos.SubGroupDtos
{
    public class SubGroupCreationalDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public DayOfWeek[] AllowedDays { get; set; }
        [Required]
        public ICollection<string> StudentsEmails { get; set; }
        [Required]
        public string CourseId { get; set; }
    }
}
