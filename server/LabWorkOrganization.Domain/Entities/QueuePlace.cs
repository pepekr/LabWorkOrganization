using LabWorkOrganization.Domain.Intefaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class QueuePlace : IHasSubroupId
    {
        [Key] public string Id { get; set; }

        public string UserId { get; set; }
        
        public string UserName { get; set; }

        [JsonIgnore] public User User { get; set; }

        [JsonIgnore] public SubGroup SubGroup { get; set; }

        public string TaskId { get; set; }

        [JsonIgnore] public LabTask Task { get; set; }

        public DateTime SpecifiedTime { get; set; }
        public string SubGroupId { get; set; }
    }
}
