using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LabWorkOrganization.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ILabTaskService, LabTaskService>();
            services.AddScoped<ISubgroupService, SubgroupService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();
            services.AddScoped<IExternalTokenService, ExternalTokenService>();
            return services;
        }
    }
}
