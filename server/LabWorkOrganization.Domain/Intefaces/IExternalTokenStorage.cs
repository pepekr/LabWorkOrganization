using LabWorkOrganization.Domain.Entities;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalTokenStorage
    {
        public Task<ExternalToken?> GetAccessTokenAsync(Guid userId, string apiName);
        public void UpdateToken(ExternalToken token);
        public void SaveToken(ExternalToken token);
        public void RemoveToken(ExternalToken token);
    }
}
