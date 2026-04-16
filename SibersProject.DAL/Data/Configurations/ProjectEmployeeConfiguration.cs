using Microsoft.EntityFrameworkCore;
using SibersProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Data.Configurations
{
    // #конфигурация_связи_проект_сотрудник / #project_employee_junction_configuration
    // Configuration for the many-to-many junction table between Project and Employee
    internal class ProjectEmployeeConfiguration : IEntityTypeConfiguration<ProjectEmployee>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProjectEmployee> builder)
        {
            builder.HasKey(pe => new {pe.ProjectId, pe.EmployeeId });
            builder.HasOne(pe => pe.Project)
                .WithMany(p => p.ProjectEmployees)
                .HasForeignKey(pe => pe.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pe => pe.Employee)
                .WithMany(e => e.ProjectEmployees)
                .HasForeignKey(pe => pe.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.ToTable("ProjectEmployees");
        }
    }
}
