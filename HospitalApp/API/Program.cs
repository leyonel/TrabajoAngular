using NLog;
using NLog.Web;
using API.Extensions;
using DataAccess.Data;
using API.Middleware;
using DataAccess.Services.Contract;
using API.Extensions.Security;
using API.Middleware.Filter;
using DataAccess.Interfaces;
using DataAccess.Repository;
using Manager.Interfaces;
using Manager.Managers;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    // Nlog setup
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    // Add services to the container.
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
        });
    });

    builder.Services.ConfigureAuthServices(builder.Configuration);
    builder.Services.AddDomainObjectServices();
    builder.Services.AddErrorServices();
    builder.Services.AddManagerServices();
    builder.Services.AddRepositoryServices();
    builder.Services.AddUserService();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IRefreshToken, RefreshTokenRepository>();
    builder.Services.AddScoped<IRefreshTokenManager, RefreshTokenManager>();

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<IDataContext, DataContext>();
    builder.Services.TestBearerTokenSwagger();
    builder.Services.AddScoped<ClientIdCheckFilter>();
    builder.Services.AddMvc();

    var app = builder.Build();
    // TODO: Check if we can extend this to keep program.cs cleaner
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseMiddleware<AdminSafeListMiddleware>(builder.Configuration["AdminSafeList"]);
    app.UseStatusCodePagesWithReExecute("/errors/{0}");
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    app.UseCors("CorsPolicy");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of Exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
