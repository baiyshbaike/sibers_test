using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SibersProject.DAL.Data.Configuration;
using SibersProject.DAL.Data.Configurations;
using SibersProject.DAL.Entities;
using SibersProject.DAL.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Data
{
    // #контекст_базы_данных / #application_db_context
    // Main EF Core database context — extends IdentityDbContext for auth support
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets / Набор данных
        public DbSet<Employee> Employees{ get; set; }
        public DbSet<Project> Projects{ get; set; }
        public DbSet<ProjectEmployee> ProjectEmployees{ get; set; }
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

            // ApplicationUser → Employee optional link / Опциональная связь пользователя с сотрудником
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Employee)
                .WithMany()
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
