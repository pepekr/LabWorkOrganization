using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Domain.Entities
{
    public class ExternalToken
    {
        [Key] public string Id { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public string AccessToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public string ApiName { get; set; }
    }
}
