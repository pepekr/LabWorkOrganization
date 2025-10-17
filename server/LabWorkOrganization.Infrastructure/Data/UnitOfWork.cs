using LabWorkOrganization.Domain.Intefaces;

namespace LabWorkOrganization.Infrastructure.Data
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public AppDbContext Context => context;
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
