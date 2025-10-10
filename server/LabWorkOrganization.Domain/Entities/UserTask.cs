using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Domain.Entities
{
    public class UserTask
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid TaskId { get; set; }
        public LabTask Task { get; set; }
        public bool IsCompleted { get; set; }
    }
}
