using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace LabWorkOrganization.Infrastructure.Auth
{
    public class GoogleTokenValidation : IExternalTokenValidation
    {
        private readonly JWKSClient _jwksClient;
        public GoogleTokenValidation(JWKSClient jwksClient)
        {
            _jwksClient = jwksClient;
        }
        public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
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
    }
}
