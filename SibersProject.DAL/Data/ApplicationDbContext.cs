using Microsoft.EntityFrameworkCore;
using SibersProject.DAL.Data.Configuration;
using SibersProject.DAL.Data.Configurations;
using SibersProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Data
{
    // #Контекст_базы_данных / #application_db_context
    // Main EF Core databse context for the application
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets / Набор данных
        public DbSet<Employee> Employees{ get; set; }
        public DbSet<Project> Projects{ get; set; }
        public DbSet<ProjectEmployee> EmployeesProjects{ get; set; }
        public DbSet<TaskItem> TaskItems{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all enity configurations from this assembly
            // Применяем все конфигурации из текущей сборки
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfigurationcs());
            modelBuilder.ApplyConfiguration(new ProjectEmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new TaskItemConfigurations());
        }
    }
}
