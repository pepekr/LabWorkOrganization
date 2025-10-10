using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Domain.Entities
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public TimeSpan LessonDuration { get; set; }
        public ICollection<User> Teachers { get; set; } = new List<User>();
        public ICollection<LabTask> Tasks { get; set; } = new List<LabTask>();
        public DateTime EndOfCourse { get; set; }
        public ICollection<SubGroup> SubGroups { get; set; } = new List<SubGroup>();
    }
}
