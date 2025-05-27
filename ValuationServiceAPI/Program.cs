using ValuationServiceAPI.Services;
using ValuationServiceAPI.Repository;
using ValuationServiceAPI.SeedData;
using NLog.Web;
using QuestPDF.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Console.WriteLine("Starting ValuationServiceAPI...");
Console.WriteLine("Seedata...");


var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);
var secretKey = builder.Configuration["Secret"];
var issuer = builder.Configuration["Issuer"];
var audience = builder.Configuration["Audience"];

// Tilføj JWT-auth
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddAuthorization();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<IValuationRepository, ValuationRepository>();
builder.Services.AddScoped<SeedData>();
builder.Services.AddScoped<IValuationService, ValuationService>();
builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IConditionReportPdfGenerator, ConditionReportPdfGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeedData>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
