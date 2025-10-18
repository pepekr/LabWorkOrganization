using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string? SubGoogleId { get; set; }
        public Role? Role { get; set; } // e.g., "Student", "Teacher", "Admin"
        [JsonIgnore]
        public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
        [JsonIgnore]
        public ICollection<SubGroup> SubGroups { get; set; } = new List<SubGroup>();
        [JsonIgnore]
        public ICollection<ExternalToken> ExternalTokens { get; set; } = new List<ExternalToken>();
    }

}
