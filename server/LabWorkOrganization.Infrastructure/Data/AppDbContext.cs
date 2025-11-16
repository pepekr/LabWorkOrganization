using LabWorkOrganization.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabWorkOrganization.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        /* migrations commands so i dont need to look 4 them elsewhere
         dotnet ef migrations remove --project .\server\LabWorkOrganization.Infrastructure --startup-project .\server\LabWorkOrganization.API
         dotnet ef migrations add migration00101010 --project .\server\LabWorkOrganization.Infrastructure --startup-project .\server\LabWorkOrganization.API
         dotnet ef database update --project .\server\LabWorkOrganization.Infrastructure --startup-project .\server\LabWorkOrganization.API 
         */
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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // no cascades for sql server (idk is it work properly)
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                modelBuilder.Entity<QueuePlace>()
                    .HasOne(q => q.Task)
                    .WithMany()
                    .HasForeignKey(q => q.TaskId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<QueuePlace>()
                    .HasOne(q => q.User)
                    .WithMany()
                    .HasForeignKey(q => q.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<QueuePlace>()
                    .HasOne(q => q.SubGroup)
                    .WithMany()
                    .HasForeignKey(q => q.SubGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
                modelBuilder.Entity<Course>()
                    .HasOne(c => c.Owner)
                    .WithMany()
                    .HasForeignKey(c => c.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
