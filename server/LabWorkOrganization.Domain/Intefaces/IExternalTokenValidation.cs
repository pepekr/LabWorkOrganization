using System.Security.Claims;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalTokenValidation
    {
        public Task<ClaimsPrincipal?> ValidateJwtTokenAsync(string token);
        public Task<ClaimsPrincipal?> ValidateOpaqueTokenAsync(string token);
    }
}
