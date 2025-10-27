using LabWorkOrganization.Domain.Intefaces;
using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Domain.Entities
{
    public class Course : IHasOwnerId
    {
        [Key]
        public string Id { get; set; }
        public string? ExternalId { get; set; }

        public string OwnerId { get; set; }
        public string? OwnerExternalId { get; set; } = null!;
        public User Owner { get; set; } = null!;
        public string Name { get; set; } = null!;
        public TimeSpan LessonDuration { get; set; }
        public ICollection<User> Teachers { get; set; } = new List<User>();
        public ICollection<LabTask> Tasks { get; set; } = new List<LabTask>();
        public DateTime EndOfCourse { get; set; }
        public ICollection<SubGroup> SubGroups { get; set; } = new List<SubGroup>();
    }
}
