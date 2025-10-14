namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ICourseScopedRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class, IHasCourseId
    {
        public Task<IEnumerable<TEntity>> GetAllByCourseIdAsync(Guid courseId);
    }
}
