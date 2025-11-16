using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace LabWorkOrganization.Infrastructure.Auth
{
    // Service for validating Google OAuth tokens (JWT and opaque tokens)
    public class GoogleTokenValidation : IExternalTokenValidation
    {
        private readonly JWKSClient _jwksClient; // Client for fetching Google's public keys

        public GoogleTokenValidation(JWKSClient jwksClient)
        {
            _jwksClient = jwksClient; // Injected via DI
        }

        // Validates a JWT token issued by Google
        public async Task<ClaimsPrincipal?> ValidateJwtTokenAsync(string token)
        {
            // Fetch public keys from Google for signature validation
            IEnumerable<SecurityKey> keys = await _jwksClient.GetGoogleKeysAsync();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, // Ensure token comes from Google
                ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },

                ValidateAudience = true, // Ensure token is meant for our application
                ValidAudience = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
                                ?? throw new Exception("Google CLIENT ID not provided"),

                ValidateIssuerSigningKey = true, // Verify token signature
                IssuerSigningKeys = keys, // Keys used to verify the signature

                ValidateLifetime = true, // Ensure token is not expired
                ClockSkew = TimeSpan.FromMinutes(5) // Allow minor clock differences
            };

            JwtSecurityTokenHandler handler = new();
            // Validate token and return claims principal
            ClaimsPrincipal principal = handler.ValidateToken(token, validationParameters, out _);

            return principal;
        }

        // Validates an opaque token (non-JWT) by calling Google's tokeninfo endpoint
        public async Task<ClaimsPrincipal?> ValidateOpaqueTokenAsync(string token, string tokenType)
        {
            using HttpClient httpClient = new(); // Consider reusing HttpClient for efficiency
            HttpResponseMessage result = await httpClient.GetAsync(
                $"https://www.googleapis.com/oauth2/v3/tokeninfo?{tokenType}={token}");

            if (!result.IsSuccessStatusCode)
                throw new SecurityTokenException("Invalid token"); // Token is invalid or expired

            string json = await result.Content.ReadAsStringAsync();
            Dictionary<string, string>? tokenInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            if (tokenInfo is null)
                throw new SecurityTokenException("Invalid token"); // Deserialization failed

            // Create claims based on token info returned from Google
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, tokenInfo.GetValueOrDefault("sub", "")),
                new Claim(ClaimTypes.Email, tokenInfo.GetValueOrDefault("email", "")),
                new Claim("email_verified", tokenInfo.GetValueOrDefault("email_verified", "false")),
                new Claim("scope", tokenInfo.GetValueOrDefault("scope", ""))
            };

            var identity = new ClaimsIdentity(claims, "GoogleToken"); // Identity using claims
            var principal = new ClaimsPrincipal(identity); // Principal representing the user

            return principal;
        }
    }
}
