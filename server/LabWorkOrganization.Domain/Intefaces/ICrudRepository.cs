using System.Linq.Expressions;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ICrudRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(string id, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> GetAllAsync();
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
