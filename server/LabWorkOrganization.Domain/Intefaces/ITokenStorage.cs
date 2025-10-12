using LabWorkOrganization.Domain.Entities;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ITokenStorage
    {
        public Task<ExternalToken?> GetAccessTokenAsync(Guid userId, string apiName);
        public void SaveToken(ExternalToken token);

    }
}
