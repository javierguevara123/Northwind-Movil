using Microsoft.Extensions.DependencyInjection;
using NorthWind.Entities.Interfaces;

namespace NorthWind.UserServices
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddUserServices(
       this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IUserService, UserService>();
            return services;
        }
    }
}
