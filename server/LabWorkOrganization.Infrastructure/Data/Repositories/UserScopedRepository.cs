using LabWorkOrganization.Domain.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace LabWorkOrganization.Infrastructure.Data.Repositories
{
    public class UserScopedRepository<TEntity> : GenericRepo<TEntity>, IUserScopedRepository<TEntity>
        where TEntity : class, IHasOwnerId
    {
        public UserScopedRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TEntity>> GetAllByUserIdAsync(string userId)
        {
            return await _dbSet.Where(t => t.OwnerId == userId).ToListAsync();
        }
    }
}
