using Microsoft.EntityFrameworkCore;

namespace LabWorkOrganization.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext
    {
    }
}
