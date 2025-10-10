using LabWorkOrganization.Application;
using LabWorkOrganization.Infrastructure;

namespace LabWorkOrganization.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDI().AddInfrastructureDI(configuration);
            return services;
        }
    }
}
