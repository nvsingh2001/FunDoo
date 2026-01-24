using DataLogic.Data;
using FunDooApp.Extensions;
using FunDooApp.Middleware;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using StackExchange.Redis;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Initializing application");

try
{

    var builder = WebApplication.CreateBuilder(args);
    
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();

    builder.Services.AddAuthorization();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.CommandTimeout(30))
    );

    builder.Services.AddJwtAuthentication(builder.Configuration);

    builder.Services.AddConsumer(builder.Configuration);

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost";
        options.InstanceName = "FunDoo_";
        options.ConfigurationOptions = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            EndPoints = { options.Configuration }
        };
    });

    builder.Services.RegisterServices();

    builder.Services.AddAutoMapperProfile();

    builder.Services.AddLogging(config =>
    {
        config.AddConsole();
        config.AddDebug();
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}