namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ICourseScopedRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class
    {
        public Task<IEnumerable<TEntity>> GetAllByCourseIdAsync(Guid courseId);
    }
}
