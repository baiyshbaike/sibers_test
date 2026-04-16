using SibersProject.BLL.DTOs.Task;
using SibersProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Mappings
{
    // #маппинг_задачи / #task_mappings
    public static class TaskMappings
    {
        public static TaskItemDto ToDto(this TaskItem task) => new()
        {
            Id = task.Id,
            Name = task.Name,
            Comment = task.Comment,
            Priority = task.Priority,
            Status = task.Status,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name ?? string.Empty,
            Author = task.Author?.ToDto(),
            Executor = task.Executor?.ToDto()
        };

        public static TaskItem ToEntity(this CreateTaskItemDto dto) => new()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Comment = dto.Comment,
            Priority = dto.Priority,
            Status = dto.Status,
            ProjectId = dto.ProjectId,
            AuthorId = dto.AuthorId,
            ExecutorId = dto.ExecutorId
        };

        public static void ApplyUpdate(this TaskItem task, UpdateTaskItemDto dto)
        {
            task.Name = dto.Name;
            task.Comment = dto.Comment;
            task.Priority = dto.Priority;
            task.Status = dto.Status;
            task.ExecutorId = dto.ExecutorId;
        }
    }
}
