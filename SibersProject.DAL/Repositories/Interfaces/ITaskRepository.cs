using SibersProject.DAL.Entities;
using SibersProject.DAL.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Repositories.Interfaces
{
    // #интерфейс_репозитория_задач / #task_repository_interface
    public interface ITaskRepository : IRepository<TaskItem>
    {
        // Get task with related Data / Получить задачу со связанными данными
        Task<TaskItem?> GetWithDetailsAsync(Guid id);

        // Get filtered and sorted tasks / Получить отфильтрованные задачи
        Task<IEnumerable<TaskItem>> GetFilteredAsync(TaskFilter filter);
    }
}
