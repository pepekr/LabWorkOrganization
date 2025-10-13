using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string SubGoogleId { get; set; }
        public Role Role { get; set; } // e.g., "Student", "Teacher", "Admin"

        public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
        public ICollection<SubGroup> SubGroups { get; set; } = new List<SubGroup>();
        public ICollection<ExternalToken> ExternalTokens { get; set; } = new List<ExternalToken>();
    }

}
