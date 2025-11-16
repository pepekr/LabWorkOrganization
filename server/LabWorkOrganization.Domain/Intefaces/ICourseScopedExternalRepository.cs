namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ICourseScopedExternalRepository<TEntity> : IExternalCrudRepo<TEntity>
        where TEntity : class, IHasCourseId
    {
        Task<IEnumerable<TEntity>> GetAllByCourseIdAsync(string courseId);
    }
}
