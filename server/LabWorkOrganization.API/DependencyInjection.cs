using LabWorkOrganization.Application;
using LabWorkOrganization.Infrastructure;

namespace LabWorkOrganization.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {
            services.AddApplicationDI().AddInfrastructureDI();
            return services;
        }
    }
}
