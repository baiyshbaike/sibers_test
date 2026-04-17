using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SibersProject.DAL.Data;
using SibersProject.DAL.Entities.Identity;
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
            // register EF Core with SQL Server / Регистрируем EF Core с SQL Server
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            // Register ASP.NET Core Identity / Регистрируем Identity
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Register repositories / Регистрируем репозитории
            services.AddScoped<IEmployeeRepository,EmployeeRepository>();
            services.AddScoped<IProjectRepository,ProjectRepository>();
            services.AddScoped<ITaskRepository,TaskRepository>();
            
            return services;
        }
    }
}
