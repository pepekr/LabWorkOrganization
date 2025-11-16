using LabWorkOrganization.Domain.Entities;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalTokenStorage
    {
        Task<ExternalToken?> GetAccessTokenAsync(string userId, string apiName);
        void UpdateToken(ExternalToken token);
        void SaveToken(ExternalToken token);
        void RemoveToken(ExternalToken token);
    }
}
