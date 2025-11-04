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
            IEnumerable<SecurityKey> keys = await _jwksClient.GetGoogleKeysAsync();


            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },
                ValidateAudience = true,
                ValidAudience =
                    Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ??
                    throw new Exception("Google id wasnt provided"),
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            JwtSecurityTokenHandler handler = new();

            ClaimsPrincipal? principal = handler.ValidateToken(token, validationParameters, out _);
            return principal;
        }

        public async Task<ClaimsPrincipal?> ValidateOpaqueTokenAsync(string token, string tokenType)
        {
            HttpClient httpClient = new();
            HttpResponseMessage result =
                await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?{tokenType}={token}");
            if (!result.IsSuccessStatusCode)
            {
                throw new SecurityTokenException("Invalid token");
            }

            string json = await result.Content.ReadAsStringAsync();
            Dictionary<string, string>? tokenInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (tokenInfo is null)
            {
                throw new SecurityTokenException("Invalid token");
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, tokenInfo.GetValueOrDefault("sub", "")),
                new Claim(ClaimTypes.Email, tokenInfo.GetValueOrDefault("email", "")),
                new Claim("email_verified", tokenInfo.GetValueOrDefault("email_verified", "false")),
                new Claim("scope", tokenInfo.GetValueOrDefault("scope", ""))
            };

            ClaimsIdentity identity = new(claims, "GoogleToken");
            ClaimsPrincipal principal = new(identity);
            return principal;
        }
    }
}
