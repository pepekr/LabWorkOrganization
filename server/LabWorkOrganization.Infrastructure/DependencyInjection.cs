using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Auth;
using LabWorkOrganization.Infrastructure.Data;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs;
using LabWorkOrganization.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LabWorkOrganization.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services,
            IConfiguration configuration)
        {
            string? dbProvider = configuration.GetValue<string>("Database");
            services.AddDbContext<AppDbContext>(options =>
            {
                switch (dbProvider)
                {
                    case "SqlServer":
                        options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
                        break;

                    case "Postgres":
                        options.UseNpgsql(configuration.GetConnectionString("Postgres"));
                        break;

                    case "Sqlite":
                        options.UseSqlite(configuration.GetConnectionString("Sqlite"));
                        break;

                    case "InMemory":
                        options.UseInMemoryDatabase("InMemoryDb");
                        break;

                    default:
                        throw new Exception($"Unsupported database provider: {dbProvider}");
                }
            });

            services.AddScoped(typeof(ICrudRepository<>), typeof(GenericRepo<>));
            services.AddScoped(typeof(ICourseScopedRepository<>), typeof(CourseScopedRepository<>));
            services.AddScoped(typeof(ISubGroupScopedRepository<>), typeof(SubGroupScopedRepository<>));
            services.AddScoped(typeof(IUserScopedRepository<>), typeof(UserScopedRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            services.AddScoped<IJwtTokenManager, JwtTokenManager>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddHttpClient();
            services.AddScoped<IExternalTokenStorage, ExternalTokenStorage>();
            services.AddScoped<IExternalTokenProvider, GoogleTokenProvider>();
            services.AddScoped<IExternalTokenValidation, GoogleTokenValidation>();
            services.AddScoped<IExternalCrudRepoFactory, ExternalCrudRepoFactory>();

            return services;
        }
    }
}
