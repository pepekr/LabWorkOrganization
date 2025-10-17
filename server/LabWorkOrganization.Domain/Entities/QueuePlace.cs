
using LabWorkOrganization.Domain.Intefaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class QueuePlace : IHasSubroupId
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public Guid SubGroupId { get; set; }
        [JsonIgnore]
        public SubGroup SubGroup { get; set; }

        public DateTime SpecifiedTime { get; set; }
    }

}
