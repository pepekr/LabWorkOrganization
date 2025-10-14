using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabWorkOrganization.Domain.Entities
{
    public class ExternalToken
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public string ApiName { get; set; }


    }
}
