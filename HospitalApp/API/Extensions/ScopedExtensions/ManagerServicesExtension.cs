using Manager.Interfaces;
using Manager.Managers;

namespace API.Extensions
{
    public static class ManagerServicesExtension
    {
        public static IServiceCollection AddManagerServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthManager, AuthManager>();
            return services;
        }
    }
}
