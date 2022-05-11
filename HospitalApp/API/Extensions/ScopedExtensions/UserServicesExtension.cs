using API.Services.UserService;

namespace API.Extensions.Security
{
    public static class UserServicesExtension
    {
        public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
