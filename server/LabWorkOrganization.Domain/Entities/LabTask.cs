using LabWorkOrganization.Domain.Intefaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class LabTask : IHasCourseId
    {
        [Key] public string Id { get; set; }

        public string? ExternalId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool IsSentRequired { get; set; }
        public TimeSpan TimeLimitPerStudent { get; set; }

        [JsonIgnore] public ICollection<UserTask> userTasks { get; set; } = new List<UserTask>();

        [JsonIgnore] public Course Course { get; set; }

        public string CourseId { get; set; }
    }
}
