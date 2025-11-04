namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalCrudRepo<TEntity>
        where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(string id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> UpdateAsync(TEntity entity, string externalId);
        Task DeleteAsync(string externalId);
    }
}
