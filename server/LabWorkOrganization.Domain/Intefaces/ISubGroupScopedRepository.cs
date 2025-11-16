using System.Linq.Expressions;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ISubGroupScopedRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class, IHasSubroupId
    {
        Task<IEnumerable<TEntity>> GetAllBySubGroupIdAsync(string subGroupId,
            params Expression<Func<TEntity, object>>[] includes);
    }
}
