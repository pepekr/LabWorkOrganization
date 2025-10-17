using LabWorkOrganization.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LabWorkOrganization.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<UserService>();
            services.AddScoped<CourseService>();
            services.AddScoped<LabTaskService>();
            services.AddScoped<SubgroupService>();
            services.AddScoped<AuthService>();
            services.AddScoped<ExternalAuthService>();
            services.AddScoped<ExternalTokenService>();
            return services;
        }
    }
}
