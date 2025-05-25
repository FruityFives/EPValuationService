using ValuationServiceAPI.Services;
using ValuationServiceAPI.Repository;
using NLog.Web;
using QuestPDF.Infrastructure;

var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
logger.Info("Starter ValuationServiceAPI...");

var builder = WebApplication.CreateBuilder(args);

// Licens til PDF-generering
QuestPDF.Settings.License = LicenseType.Community;

// Konfigurer logging med NLog
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Dependency Injection
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<IValuationRepository, ValuationRepository>();
builder.Services.AddScoped<IValuationService, ValuationService>();
builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IConditionReportPdfGenerator, ConditionReportPdfGenerator>();

// API Controllers og Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger vises kun i development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // God praksis, især udenfor Docker
app.UseAuthorization();

app.MapControllers();

app.Run();
