using LabWorkOrganization.Domain.Intefaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace LabWorkOrganization.Infrastructure.Data.Repositories
{
    public class SubGroupScopedRepository<TEntity> : GenericRepo<TEntity>, ISubGroupScopedRepository<TEntity>
        where TEntity : class, IHasSubroupId
    {
        public SubGroupScopedRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TEntity>> GetAllBySubGroupIdAsync(
    string subGroupId,
    params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;


            foreach (var include in includes)
                query = query.Include(include);


            return await query
                .Where(t => EF.Property<string>(t, "SubGroupId") == subGroupId)
                .ToListAsync();
        }

    }
}
