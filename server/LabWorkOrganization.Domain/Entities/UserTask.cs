using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class UserTask
    {
        [Key] public string Id { get; set; }

        public string UserId { get; set; }

        [JsonIgnore] public User User { get; set; }

        public string TaskId { get; set; }

        [JsonIgnore] public LabTask Task { get; set; }

        public bool IsCompleted { get; set; }
    }
}
