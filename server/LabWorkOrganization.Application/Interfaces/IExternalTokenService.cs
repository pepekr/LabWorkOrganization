using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface IExternalTokenService
    {
        Task<Result<string>> GetAccessTokenFromDbAsync(Guid userId, string apiName);
        Task<Result<ExternalToken>> SaveTokenAsync(Guid userId, ExternalTokenDto extTokenDto);
    }
}