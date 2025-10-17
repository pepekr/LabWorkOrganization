using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class UserTask
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public Guid TaskId { get; set; }
        [JsonIgnore]
        public LabTask Task { get; set; }
        public bool IsCompleted { get; set; }
    }
}
