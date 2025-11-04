using System.Security.Claims;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IJwtTokenManager
    {
        string GenerateJwtToken(string email, string id, string? externalServiceId, int expirationInMinutes);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
