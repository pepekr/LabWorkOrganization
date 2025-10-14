using LabWorkOrganization.Domain.Intefaces;
using Microsoft.AspNetCore.Http;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs
{
    public class GoogleTokenProvider : IExternalTokenProvider
    {
        private readonly IExternalTokenStorage _tokenStorage;
        private readonly string _currentUserId;

        public GoogleTokenProvider(IExternalTokenStorage tokenStorage, IHttpContextAccessor ctx)
        {
            _tokenStorage = tokenStorage;
            _currentUserId = ctx.HttpContext?.User?.FindFirst("id")?.Value ?? "";
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var token = await _tokenStorage.GetAccessTokenAsync(Guid.Parse(_currentUserId), "Google");
            if (token == null) throw new Exception("No access token found");
            return token.AccessToken;
        }
    }

}
