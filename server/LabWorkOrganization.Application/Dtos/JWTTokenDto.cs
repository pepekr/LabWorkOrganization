using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos
{
    public class JWTTokenDto
    {
        [Required] public string AccessToken { get; set; }

        [Required] public string RefreshToken { get; set; }
    }
}
