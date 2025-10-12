using System.Security.Claims;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ITokenValidation
    {
        public Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
    }
}
