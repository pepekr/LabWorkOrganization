using Google.Apis.Auth.OAuth2.Responses;
using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Utilities;
using System.Security.Claims;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface IExternalAuthService
    {
        Task<TokenResponse> GetGoogleTokensAsync(string code);
        Task<Result<JWTTokenDto>> HandleExternalAuth(string code, ClaimsPrincipal? claimsPrincipal);
        string RedirectUri();
    }
}