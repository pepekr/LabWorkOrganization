namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalCrudRepo<TEntity>
   where TEntity : class
    {
        public Task<TEntity> AddAsync(TEntity entity);
        public Task<TEntity?> GetByIdAsync(Guid id);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public void Update(TEntity entity);
        public void Delete(TEntity entity);
    }
}
