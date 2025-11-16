using Google.Apis.Auth.OAuth2.Responses;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs
{
    public class GoogleTokenProvider : IExternalTokenProvider
    {
        private readonly string _currentUserId;
        private readonly HttpClient _httpClient;
        private readonly IExternalTokenStorage _tokenStorage;

        public GoogleTokenProvider(IExternalTokenStorage tokenStorage, IHttpContextAccessor ctx, HttpClient client)
        {
            _httpClient = client;
            _tokenStorage = tokenStorage;
            _currentUserId = ctx.HttpContext?.User?.FindFirst("id")?.Value ?? "";
        }

        public async Task<string> GetAccessTokenAsync()
        {
            ExternalToken? token = await _tokenStorage.GetAccessTokenAsync(_currentUserId, "Google");
            if (token == null)
            {
                throw new Exception("No access token found");
            }

            return token.AccessToken;
        }

        public async Task<string> HandleRefreshAsync(string refreshToken)
        {
            FormUrlEncodedContent content = new(new[]
            {
                new KeyValuePair<string, string>("client_id",
                    Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")!),
                new KeyValuePair<string, string>("client_secret",
                    Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")!),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            });
            HttpResponseMessage response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to refresh token");
            }

            string responseContent = await response.Content.ReadAsStringAsync();
            TokenResponse? tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
            if (tokenResponse == null || tokenResponse.AccessToken == null)
            {
                throw new Exception("Invalid token response");
            }

            return tokenResponse.AccessToken;
        }
    }
}
