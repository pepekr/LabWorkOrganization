using System.Security.Claims;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalTokenValidation
    {
        Task<ClaimsPrincipal?> ValidateJwtTokenAsync(string token);
        Task<ClaimsPrincipal?> ValidateOpaqueTokenAsync(string token, string tokenType);
    }
}
