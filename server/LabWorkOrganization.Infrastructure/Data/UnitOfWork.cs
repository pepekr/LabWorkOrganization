namespace LabWorkOrganization.Infrastructure.Data
{
    public class UnitOfWork(AppDbContext context)
    {
        public AppDbContext Context => context;
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
