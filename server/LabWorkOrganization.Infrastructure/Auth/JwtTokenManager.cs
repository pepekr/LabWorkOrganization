using LabWorkOrganization.Domain.Intefaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LabWorkOrganization.Infrastructure.Auth
{
    public class JwtTokenManager : IJwtTokenManager
    {
        public string GenerateJwtToken(string email, string id, string? externalServiceId, int expirationInMinutes)
        {
            string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")!;
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("email", email), new Claim("id", id),
                    new Claim("externalId", externalServiceId ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials = credentials,
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            };
            JsonWebTokenHandler tokenHandler = new();
            string token = tokenHandler.CreateToken(tokenDescriptor);
            return token;
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);


            ClaimsPrincipal? principal = tokenHandler.ValidateToken(token,
                new TokenValidationParameters
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
