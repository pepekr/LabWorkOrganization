using LabWorkOrganization.Domain.Intefaces;
using Microsoft.EntityFrameworkCore;
namespace LabWorkOrganization.Infrastructure.Data.Repositories
{
    public class SubGroupScopedRepository<TEntity> : GenericRepo<TEntity>, ISubGroupScopedRepository<TEntity>
        where TEntity : class, IHasSubroupId
    {
        public SubGroupScopedRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TEntity>> GetAllBySubGroupIdAsync(string subGroupId)
        {
            return await _dbSet.Where(t => t.SubGroupId == subGroupId).ToListAsync();
        }
    }
}
