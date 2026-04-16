using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SibersProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Data.Configurations
{

    // #конфигурация_задаяи / #taskitem_ef_configuration
    // Fluent API configuration for TaskTiem entity.
    internal class TaskItemConfigurations : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(300);
            builder.Property(t => t.Comment).HasMaxLength(1000);

            // Task belongs to a project / Задача принадлежит проекту
            builder.HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task author - no cascade delete / Автор задачи - без каскадного удаления
            builder.HasOne(t => t.Author)
                .WithMany(e => e.AuthoredTasks)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Task executor (optional) - no cascade / Исполнитель (Опционально)
            builder.HasOne(t => t.Executor)
                .WithMany(e => e.AssignedTasks)
                .HasForeignKey(t => t.ExecutorId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.ToTable("TaskItems");

        }
    }
}
