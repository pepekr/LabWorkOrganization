namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalCrudRepo<TEntity>
   where TEntity : class
    {
        public Task<TEntity> AddAsync(TEntity entity);
        public Task<TEntity?> GetByIdAsync(Guid id);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<TEntity> UpdateAsync(TEntity entity, Guid externalId);
        public Task DeleteAsync(Guid externalId);
    }
}
