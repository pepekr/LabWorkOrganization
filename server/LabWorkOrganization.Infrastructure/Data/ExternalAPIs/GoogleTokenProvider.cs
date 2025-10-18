using LabWorkOrganization.Domain.Intefaces;
using Microsoft.AspNetCore.Http;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs
{
    public class GoogleTokenProvider : IExternalTokenProvider
    {
        private readonly IExternalTokenStorage _tokenStorage;
        private readonly string _currentUserId;
        private readonly HttpClient _httpClient;
        public GoogleTokenProvider(IExternalTokenStorage tokenStorage, IHttpContextAccessor ctx, HttpClient client)
        {
            _httpClient = client;
            _tokenStorage = tokenStorage;
            _currentUserId = ctx.HttpContext?.User?.FindFirst("id")?.Value ?? "";
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var token = await _tokenStorage.GetAccessTokenAsync(_currentUserId, "Google");
            if (token == null) throw new Exception("No access token found");
            return token.AccessToken;
        }

        public async Task<string> HandleRefreshAsync(string refreshToken)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")!),
                new KeyValuePair<string, string>("client_secret", Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")!),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            });
            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to refresh token");
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Google.Apis.Auth.OAuth2.Responses.TokenResponse>(responseContent);
            if (tokenResponse == null || tokenResponse.AccessToken == null)
            {
                throw new Exception("Invalid token response");
            }

            return tokenResponse.AccessToken;
        }
    }

}
