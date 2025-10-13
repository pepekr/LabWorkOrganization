using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos
{
    public class SubGroupCreationalDto
    {

        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public DayOfWeek[] AllowedDays { get; set; }

        [Required]
        public ICollection<Guid> Students { get; set; } = new List<Guid>();
        [Required]
        public Guid CourseId { get; set; }
    }
}
