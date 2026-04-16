using SibersProject.BLL.DTOs.Task;
using SibersProject.BLL.Services.Interfaces;
using SibersProject.DAL.Filters;
using SibersProject.DAL.Repositories.Interfaces;
using SibersProject.BLL.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Services
{
    // #сервис_задач / #task_service
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskService(
            ITaskRepository taskRepository,
            IEmployeeRepository employeeRepository,
            IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<TaskItemDto>> GetAllAsync(TaskFilter filter)
        {
            var tasks = await _taskRepository.GetFilteredAsync(filter);
            return tasks.Select(t => t.ToDto());
        }

        public async Task<TaskItemDto?> GetByIdAsync(Guid id)
        {
            var task = await _taskRepository.GetWithDetailsAsync(id);
            return task?.ToDto();
        }

        public async Task<TaskItemDto> CreateAsync(CreateTaskItemDto dto)
        {
            _ = await _projectRepository.GetByIdAsync(dto.ProjectId)
                ?? throw new KeyNotFoundException($"Project with id '{dto.ProjectId}' not found.");

            _ = await _employeeRepository.GetByIdAsync(dto.AuthorId)
                ?? throw new KeyNotFoundException($"Author employee with id '{dto.AuthorId}' not found.");

            if (dto.ExecutorId.HasValue)
            {
                _ = await _employeeRepository.GetByIdAsync(dto.ExecutorId.Value)
                    ?? throw new KeyNotFoundException($"Executor employee with id '{dto.ExecutorId}' not found.");
            }

            var entity = dto.ToEntity();
            await _taskRepository.CreateAsync(entity);

            var detailed = await _taskRepository.GetWithDetailsAsync(entity.Id);
            return detailed!.ToDto();
        }

        public async Task<TaskItemDto> UpdateAsync(Guid id, UpdateTaskItemDto dto)
        {
            var task = await _taskRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Task with id '{id}' not found.");

            if (dto.ExecutorId.HasValue)
            {
                _ = await _employeeRepository.GetByIdAsync(dto.ExecutorId.Value)
                    ?? throw new KeyNotFoundException($"Executor employee with id '{dto.ExecutorId}' not found.");
            }

            task.ApplyUpdate(dto);
            await _taskRepository.UpdateAsync(task);

            var detailed = await _taskRepository.GetWithDetailsAsync(id);
            return detailed!.ToDto();
        }

        public async Task DeleteAsync(Guid id)
        {
            _ = await _taskRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Task with id '{id}' not found.");

            await _taskRepository.DeleteAsync(id);
        }
    }
}
