using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabWorkOrganization.Infrastructure.Auth
{
    // Handles CRUD operations for external tokens stored in the database.
    public class ExternalTokenStorage : IExternalTokenStorage
    {
        private readonly AppDbContext _context; // DbContext injected via DI

        public ExternalTokenStorage(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves a token for a specific user and API
        public async Task<ExternalToken?> GetAccessTokenAsync(string userId, string apiName)
        {
            return await _context.ExternalTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.ApiName == apiName);
        }

        // Updates an existing token (does not save changes immediately)
        public void UpdateToken(ExternalToken token)
        {
            _context.ExternalTokens.Update(token);
        }

        // Adds a new token to the database (does not save changes immediately)
        public void SaveToken(ExternalToken token)
        {
            _context.ExternalTokens.Add(token);
        }

        // Removes a token from the database (does not save changes immediately)
        public void RemoveToken(ExternalToken token)
        {
            _context.ExternalTokens.Remove(token);
        }
    }
}
