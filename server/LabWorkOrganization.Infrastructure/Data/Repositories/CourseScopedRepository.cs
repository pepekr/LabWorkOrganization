using LabWorkOrganization.Domain.Intefaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LabWorkOrganization.Infrastructure.Data.Repositories
{
    public class CourseScopedRepository<TEntity> : GenericRepo<TEntity>, ICourseScopedRepository<TEntity>
        where TEntity : class, IHasCourseId
    {
        public CourseScopedRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TEntity>> GetAllByCourseIdAsync(
            string courseId,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;


            foreach (Expression<Func<TEntity, object>> include in includes)
            {
                query = query.Include(include);
            }


            return await query
                .Where(t => EF.Property<string>(t, "CourseId") == courseId)
                .ToListAsync();
        }
    }
}
