using System.Linq.Expressions;

namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ICourseScopedRepository<TEntity> : ICrudRepository<TEntity>
        where TEntity : class, IHasCourseId
    {
        Task<IEnumerable<TEntity>> GetAllByCourseIdAsync(string courseId,
            params Expression<Func<TEntity, object>>[] includes);
    }
}
