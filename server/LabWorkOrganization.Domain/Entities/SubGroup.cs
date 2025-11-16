using LabWorkOrganization.Domain.Intefaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class SubGroup : IHasCourseId
    {
        [Key] public string Id { get; set; }

        public string Name { get; set; } = null!;
        public DayOfWeek[] AllowedDays { get; set; }

        [JsonIgnore] public ICollection<QueuePlace> Queue { get; set; } = new List<QueuePlace>();

        public ICollection<User> Students { get; set; } = new List<User>();

        [JsonIgnore] public Course Course { get; set; }

        public string CourseId { get; set; }
    }
}
