using Microsoft.Extensions.DependencyInjection;
using SibersProject.BLL.Services;
using SibersProject.BLL.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Extensions
{
    // #расширения_сервисов_BLL / #bll_service_extensions
    // Registers all BLL services into the DI container
    public static class BllServiceExtensions
    {
        public static IServiceCollection AddBllServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();

            return services;
        }
    }
}
