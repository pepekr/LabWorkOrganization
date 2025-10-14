using System.Security.Claims;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalTokenValidation
    {
        public Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
    }
}
