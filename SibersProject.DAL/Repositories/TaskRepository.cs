using Microsoft.EntityFrameworkCore;
using SibersProject.DAL.Data;
using SibersProject.DAL.Entities;
using SibersProject.DAL.Filters;
using SibersProject.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Repositories
{
    // #репозиторий_задач / #task_repository
    public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TaskItem?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Author)
                .Include(t => t.Executor)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskItem>> GetFilteredAsync(TaskFilter filter)
        {
            var query = _dbSet
                .Include(t => t.Author)
                .Include(t => t.Executor)
                .Include(t => t.Project)
                .AsQueryable();

            // Filter by project / Фильтр по проекту
            if (filter.ProjectId.HasValue)
                query = query.Where(t => t.ProjectId == filter.ProjectId.Value);

            // Filter by status / Фильтр по статусу
            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            // Filter by executor / Фильтр по исполнителю
            if (filter.ExecutorId.HasValue)
                query = query.Where(t => t.ExecutorId == filter.ExecutorId.Value);

            // Filter by project manager ownership / Фильтр по менеджеру проекта
            if (filter.ProjectManagerId.HasValue)
                query = query.Where(t => t.Project.ProjectManagerId == filter.ProjectManagerId.Value);

            // Apply sorting / Применяем сортировку
            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                "priority" => filter.SortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                "status" => filter.SortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                _ => query.OrderBy(t => t.Priority)
            };

            return await query.ToListAsync();
        }
    }
}
