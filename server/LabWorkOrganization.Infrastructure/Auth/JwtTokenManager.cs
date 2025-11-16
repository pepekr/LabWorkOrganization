using LabWorkOrganization.Domain.Intefaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LabWorkOrganization.Infrastructure.Auth
{
    // Service for generating and validating JWT tokens for internal authentication
    public class JwtTokenManager : IJwtTokenManager
    {
        public JwtTokenManager() { }

        // Generates a JWT token containing email, id, and optional external service id
        public string GenerateJwtToken(string email, string id, string? externalServiceId, int expirationInMinutes)
        {
            // Fetch secret key from environment variables
            string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define token payload (claims) and expiration
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("email", email),
                    new Claim("id", id),
                    new Claim("externalId", externalServiceId ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials = credentials,
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            };

            var tokenHandler = new JsonWebTokenHandler();
            string token = tokenHandler.CreateToken(tokenDescriptor);
            return token;
        }

        // Validates a JWT token and returns ClaimsPrincipal if valid
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);

            // Set validation parameters for issuer, audience, signature, and lifetime
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),

                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
    }
}
