using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Data;

namespace LabWorkOrganization.Infrastructure.Auth
{
    public class TokenStorage : ITokenStorage
    {
        AppDbContext _context;
        public TokenStorage(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ExternalToken?> GetAccessTokenAsync(Guid userId, string apiName)
        {
            return await _context.ExternalTokens.FindAsync(userId, apiName);

        }
        public void SaveToken(ExternalToken token)
        {
            _context.ExternalTokens.Update(token);
        }
        public void RemoveToken(ExternalToken token)
        {
            _context.ExternalTokens.Remove(token);
        }
    }
}
