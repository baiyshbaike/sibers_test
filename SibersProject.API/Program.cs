// #?????_????? / #application_entry_point
// Main entry point: loads config, registers services, configures middleware

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SibersProject.API.Extensions;
using SibersProject.BLL.Extensions;
using SibersProject.DAL.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load .env file for local development (outside Docker)
// ????????? .env ???? ??? ????????? ?????????? (??? Docker)
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envFile))
    DotNetEnv.Env.Load(envFile);


// Build connection string from environment variables
// ?????? ?????? ??????????? ?? ?????????? ?????????
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "SibersProjectDb";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "YourStrong@Password123";

var connectionString =
    $"Server={dbServer},{dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=True";


// Read JWT settings from env / ?????? JWT ????????? ?? ?????????? ?????????
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "DefaultSecretKeyPleaseChangeInProduction!";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "SibersAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "SibersClient";

// Register layers via extension methods / ???????????? ???? ????? ?????? ??????????
builder.Services.AddDalServices(connectionString);
builder.Services.AddBllServices();

// JWT Authentication / ?????????????? ????? JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Swagger with JWT support / Swagger ? ?????????? JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Sibers Project Manager API",
        Version = "v1",
        Description = "Backend API / API ??? ?????????? ?????????"
    });

    // Add JWT Bearer button to Swagger UI / ?????? ??????????? ? Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    // Security requirement can be added here if needed for current OpenAPI package model.
});

// CORS ? allow frontend origin / ????????? ???????? ?????????
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins(
                "http://localhost:5173",  // Vite dev server
                "http://localhost:3000"   // Docker frontend
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Apply migrations + seed identity data at startup.
// Применяем миграции и заполняем identity-данные при запуске.
await app.ApplyMigrationsAndSeedIdentityAsync();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sibers API v1");
        c.RoutePrefix = string.Empty; // Swagger at root / Swagger ?? ???????? ????
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make Program accessible for integration tests / ?????? Program ????????? ??? ??????
public partial class Program { }