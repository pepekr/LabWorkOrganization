using LabWorkOrganization.Domain.Intefaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class LabTask : IHasCourseId
    {
        [Key]
        public string Id { get; set; }
        public string? ExternalId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public bool IsSentRequired { get; set; }
        public TimeSpan TimeLimitPerStudent { get; set; }
        [JsonIgnore]
        public ICollection<UserTask> userTasks { get; set; } = new List<UserTask>();
        public string CourseId { get; set; }
        [JsonIgnore]
        public Course Course { get; set; }
    }
}
