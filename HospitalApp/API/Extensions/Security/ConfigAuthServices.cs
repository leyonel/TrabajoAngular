#region
/*  In AddAuthentication method, I am setting the default authentication and challenge scheme 
    as JwtBearerDefaults.AuthenticationScheme.
    
    The AddJwtBearer will handle all requests and will check for a valid JWT Token in the header. 
    If it is not passed, or the token is expired, it will generate a 401 Unauthorized HTTP response.
*/
#endregion

using DomainObject.Interfaces;
using DomainObject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class ConfigAuthServices
    {
        public static void ConfigureAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            var TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(1),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = TokenValidationParameters;
            });

            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.AddSingleton(TokenValidationParameters);
        }
    }
}
