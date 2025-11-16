using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Domain.Entities
{
    public class Role
    {
        [Key] public string Id { get; set; }

        public string Name { get; set; } // e.g., "Student", "Teacher", "Admin"
    }
}
