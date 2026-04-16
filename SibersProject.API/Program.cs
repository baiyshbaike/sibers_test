// #точка_входа / #application_entry_point
// Main entry point: loads config, registers services, configures middleware

using Microsoft.EntityFrameworkCore;
using SibersProject.BLL.Extensions;
using SibersProject.DAL.Data;
using SibersProject.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Load .env file for local development (outside Docker)
// Загружаем .env файл для локальной разработки (вне Docker)
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (!File.Exists(envFile))
    DotNetEnv.Env.Load(envFile);


// Build connection string from environment variables
// Строим строку подключения из переменных окружения
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "SibersProjectDb";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "YourStrong@Password123";

var connectionString =
    $"Server={dbServer},{dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=True";

// Register layers via extension methods / Регистрируем слои через методы расширения
builder.Services.AddDalServices(connectionString);
builder.Services.AddBllServices();

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Sibers Project Manager API",
        Version = "v1",
        Description = "Backend API for project and employee management / API для управления проектами и сотрудниками"
    });

    // Include XML comments for Swagger / Включаем XML комментарии для Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// CORS — allow all origins for development / Разрешаем все источники для разработки
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Apply migrations automatically on startup / Автоматически применяем миграции при запуске
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sibers API v1");
        c.RoutePrefix = string.Empty; // Swagger at root / Swagger на корневом пути
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();

app.Run();

// Make Program accessible for integration tests / Делаем Program доступным для тестов
public partial class Program { }