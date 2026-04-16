using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Data
{
    // #фабрика_контекста / #design_time_db_context_factory
    // Used by EF Core CLI tools (migrations) as design time
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Load .env file for design-time migration commands
            // Загружаем .env для команд миграции EF Core CLI
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
            if (File.Exists(envPath))
                DotNetEnv.Env.Load(envPath);
            
            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connectionString = BuildConnectionString();
            optionBuilder.UseSqlServer(connectionString);
            return new ApplicationDbContext(optionBuilder.Options);
        }
        private static string BuildConnectionString()
        {
            var server = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
            var name = Environment.GetEnvironmentVariable("DB_NAME") ?? "SibersProjectDb";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "sibers";
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "YourStrong@Password";

            return $"Server={server},{port};Database={name};User Id={user};Password={password};TrustServerCertificate=True";
        }
    }
}
