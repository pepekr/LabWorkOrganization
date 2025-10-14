namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ISubGroupScopedRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class, IHasSubroupId
    {
        public Task<IEnumerable<TEntity>> GetAllBySubGroupIdAsync(Guid subGroupId);
    }
}
