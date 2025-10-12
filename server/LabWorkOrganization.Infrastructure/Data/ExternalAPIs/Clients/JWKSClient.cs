using Microsoft.IdentityModel.Tokens;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients
{
    public class JWKSClient
    {
        private HttpClient _httpClient;
        public JWKSClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SecurityKey>> GetGoogleKeysAsync()
        {
            var json = await _httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v3/certs");
            var jwks = new JsonWebKeySet(json);
            return jwks.Keys;
        }
    }
}
