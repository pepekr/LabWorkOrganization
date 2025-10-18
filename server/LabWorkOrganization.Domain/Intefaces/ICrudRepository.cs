namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ICrudRepository<TEntity>
      where TEntity : class
    {
        public Task<TEntity> AddAsync(TEntity entity);
        public Task<TEntity?> GetByIdAsync(string id);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public void Update(TEntity entity);
        public void Delete(TEntity entity);
    }
}
