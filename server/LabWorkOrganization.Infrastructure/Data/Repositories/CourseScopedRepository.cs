using LabWorkOrganization.Domain.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace LabWorkOrganization.Infrastructure.Data.Repositories
{
    public class CourseScopedRepository<TEntity> : GenericRepo<TEntity>, ICourseScopedRepository<TEntity>
        where TEntity : class, IHasCourseId
    {
        public CourseScopedRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<TEntity>> GetAllByCourseIdAsync(Guid courseId)
        {
            return await _dbSet.Where(t => t.CourseId == courseId).ToListAsync();
        }
    }
}
