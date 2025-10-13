using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LabWorkOrganization.Infrastructure.Auth
{
    public class JwtTokenManager
    {
        public JwtTokenManager() { }
        public string GenerateJwtToken(string email, string id, string? eternalServiceId)
        {
            string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("email", email),
                    new System.Security.Claims.Claim("id", id),
                    new System.Security.Claims.Claim("externalId", eternalServiceId ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials,
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            };
            var tokenHandler = new JsonWebTokenHandler();
            string token = tokenHandler.CreateToken(tokenDescriptor);
            return token;
        }
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);


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
