using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos
{
    public class ExternalTokenDto
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public string ApiName { get; set; }

        [Required]
        public DateTime ExpiresIn { get; set; }
    }
}
