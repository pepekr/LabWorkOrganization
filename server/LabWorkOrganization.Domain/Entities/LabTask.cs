using LabWorkOrganization.Domain.Intefaces;
using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Domain.Entities
{
    public class LabTask : IHasCourseId
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ExternalId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public bool IsSentRequired { get; set; }
        public TimeSpan TimeLimitPerStudent { get; set; }

        public ICollection<UserTask> userTasks { get; set; } = new List<UserTask>();
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
