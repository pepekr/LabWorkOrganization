namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IUserScopedRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class, IHasOwnerId
    {
        public Task<IEnumerable<TEntity>> GetAllByUserIdAsync(string userId);
    }
}
