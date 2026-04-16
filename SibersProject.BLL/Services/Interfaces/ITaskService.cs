using SibersProject.BLL.DTOs.Task;
using SibersProject.DAL.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Services.Interfaces
{
    // #интерфейс_сервиса_задач / #task_service_interface
    public interface ITaskService
    {
        Task<IEnumerable<TaskItemDto>> GetAllAsync(TaskFilter filter);
        Task<TaskItemDto?> GetByIdAsync(Guid id);
        Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto);
        Task<TaskItemDto> UpdateAsync(Guid id, UpdateTaskItemDto dto);
        Task DeleteAsync(Guid id);
    }
}
