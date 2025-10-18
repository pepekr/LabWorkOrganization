using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabWorkOrganization.Infrastructure.Auth
{
    public class ExternalTokenStorage : IExternalTokenStorage
    {
        AppDbContext _context;
        public ExternalTokenStorage(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ExternalToken?> GetAccessTokenAsync(Guid userId, string apiName)
        {
            return await _context.ExternalTokens
     .FirstOrDefaultAsync(t => t.UserId == userId && t.ApiName == apiName);

        }
        public void UpdateToken(ExternalToken token)
        {
            _context.ExternalTokens.Update(token);
        }
        public void SaveToken(ExternalToken token)
        {
            _context.ExternalTokens.Add(token);
        }
        public void RemoveToken(ExternalToken token)
        {
            _context.ExternalTokens.Remove(token);
        }
    }
}
