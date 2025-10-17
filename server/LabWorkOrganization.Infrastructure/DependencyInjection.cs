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
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(ICrudRepository<>), typeof(GenericRepo<>));
            services.AddScoped(typeof(ICourseScopedRepository<>), typeof(CourseScopedRepository<>));
            services.AddScoped(typeof(ISubGroupScopedRepository<>), typeof(SubGroupScopedRepository<>));
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
