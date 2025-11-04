namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IUserScopedRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class, IHasOwnerId
    {
        Task<IEnumerable<TEntity>> GetAllByUserIdAsync(string userId);
    }
}
