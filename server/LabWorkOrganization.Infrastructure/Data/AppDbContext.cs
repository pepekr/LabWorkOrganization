using LabWorkOrganization.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace LabWorkOrganization.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }
        public DbSet<ExternalToken> ExternalTokens { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SubGroup> SubGroups { get; set; }
        public DbSet<QueuePlace> QueuePlaces { get; set; }
        public DbSet<LabTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
        new Role { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Student" },
        new Role { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Teacher" },
        new Role { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Admin" }
    );
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        }
    }
}
