using DomainObject.DomainObjects;
using DomainObject.Interfaces;

namespace API.Extensions
{
    public static class DomainObjectServicesExtension
    {
        public static IServiceCollection AddDomainObjectServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthDomainObject, AuthDomainObject>();
            return services;
        }
    }
}
