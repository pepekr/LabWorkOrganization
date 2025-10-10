

using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Domain.Entities
{
    public class SubGroup
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DayOfWeek[] AllowedDays { get; set; }
        public ICollection<QueuePlace> Queue { get; set; } = new List<QueuePlace>();
        public ICollection<User> Students { get; set; } = new List<User>();
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
