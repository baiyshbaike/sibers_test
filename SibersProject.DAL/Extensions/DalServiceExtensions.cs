using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SibersProject.DAL.Data;
using SibersProject.DAL.Repositories;
using SibersProject.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Extensions
{
    // #расширения_сервисов_DAL / #dal_service_extensions
    // Registers all DAL services into the DI container
    public static class DalServiceExtensions
    {
        public static IServiceCollection AddDalServices(
            this IServiceCollection services,
            string connectionString)
        {
            // register EF Core with SQL Server / Регисттрируем EF Core с SQL Server
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            // Register repositories / Регистрируем репозитории
            services.AddScoped<IEmployeeRepository,EmployeeRepository>();
            services.AddScoped<IProjectRepository,ProjectRepository>();
            services.AddScoped<ITaskRepository,TaskRepository>();
            
            return services;
        }
    }
}
