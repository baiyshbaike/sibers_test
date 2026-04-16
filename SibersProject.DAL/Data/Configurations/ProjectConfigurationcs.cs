using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SibersProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Data.Configurations
{
    // #конфигурация_проекта / #project_ef_configuration
    // Fluent API configuration for Project entity
    public class ProjectConfigurationcs : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.CustomerCompany).IsRequired().HasMaxLength(200);
            builder.Property(p => p.ExecutorCompany).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Priority).IsRequired();

            // Project Manager relation: restrcit delete to prevent accidental cascade
            // Связь с руководителем: запрет каскадного удаления
            builder.HasOne(p => p.ProjectManager)
                .WithMany(e => e.ManagedProjects)
                .HasForeignKey(p => p.ProjectManagerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.ToTable("Projects");
        }
    }
}
