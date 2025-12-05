using Microsoft.Extensions.DependencyInjection;
using NorthWind.DomainLogs.Entities.Interfaces;
using NorthWind.DomainLogs.Entities.Services;

namespace NorthWind.DomainLogs.Entities
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddDomainLogsServices(
       this IServiceCollection services)
        {
            services.AddScoped<IDomainLogger, DomainLogger>();
            return services;
        }
    }

}
