using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Builder;
using ValuationServiceAPI.Services;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
logger.Debug("Starting ValuationService");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<IMongoClient>(s =>
        new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));
    builder.Services.AddScoped<ValuationService>();
    builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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
    LogManager.Shutdown();
}

