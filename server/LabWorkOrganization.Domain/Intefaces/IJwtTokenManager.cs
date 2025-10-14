namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IJwtTokenManager
    {
        public string GenerateJwtToken(string email, string id, string? externalServiceId, int expirationInMinutes);
        public System.Security.Claims.ClaimsPrincipal? ValidateToken(string token);

    }
}
