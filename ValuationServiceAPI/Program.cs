using ValuationServiceAPI.Services;
using ValuationServiceAPI.Repository;
using NLog.Web;
using QuestPDF.Infrastructure;

Console.WriteLine("Starting ValuationServiceAPI...");

var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<IValuationRepository, ValuationRepository>();
builder.Services.AddScoped<IValuationService, ValuationService>();
builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IConditionReportPdfGenerator, ConditionReportPdfGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
