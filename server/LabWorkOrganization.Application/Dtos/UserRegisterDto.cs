using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
