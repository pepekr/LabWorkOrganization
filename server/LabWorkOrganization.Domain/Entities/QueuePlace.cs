
using LabWorkOrganization.Domain.Intefaces;
using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Domain.Entities
{
    public class QueuePlace : IHasSubroupId
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid SubGroupId { get; set; }
        public SubGroup SubGroup { get; set; }

        public DateTime SpecifiedTime { get; set; }
    }

}
