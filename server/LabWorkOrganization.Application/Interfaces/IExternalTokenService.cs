using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface IExternalTokenService
    {
        Task<Result<string>> GetAccessTokenFromDbAsync(string userId, string apiName);
        Task<Result<ExternalToken>> SaveTokenAsync(string userId, ExternalTokenDto extTokenDto);
        Task<Result<ExternalToken>> GetRefreshedToken(string refreshToken, string userId, string apiName);
        Task<Result<bool>> RemoveTokenAsync(string userId, string apiName);
    }
}
