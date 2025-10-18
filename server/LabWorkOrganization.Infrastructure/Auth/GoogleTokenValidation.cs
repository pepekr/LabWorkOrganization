using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
namespace LabWorkOrganization.Infrastructure.Auth
{
    public class GoogleTokenValidation : IExternalTokenValidation
    {
        private readonly JWKSClient _jwksClient;
        public GoogleTokenValidation(JWKSClient jwksClient)
        {
            _jwksClient = jwksClient;
        }
        public async Task<ClaimsPrincipal?> ValidateJwtTokenAsync(string token)
        {
            var keys = await _jwksClient.GetGoogleKeysAsync();


            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },

                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? throw new Exception("Google id wasnt provided"),

                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var handler = new JwtSecurityTokenHandler();

            var principal = handler.ValidateToken(token, validationParameters, out _);
            return principal;
        }

        public async Task<ClaimsPrincipal?> ValidateOpaqueTokenAsync(string token)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={token}");
            if (!result.IsSuccessStatusCode)
            {
                throw new SecurityTokenException("Invalid token");
            }
            string json = await result.Content.ReadAsStringAsync();
            var tokenInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (tokenInfo is null)
            {
                throw new SecurityTokenException("Invalid token");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, tokenInfo.GetValueOrDefault("sub", "")),
                new Claim(ClaimTypes.Email, tokenInfo.GetValueOrDefault("email", "")),
                new Claim("email_verified", tokenInfo.GetValueOrDefault("email_verified", "false")),
                new Claim("scope", tokenInfo.GetValueOrDefault("scope", ""))
            };

            var identity = new ClaimsIdentity(claims, "GoogleToken");
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
