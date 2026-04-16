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
    // #репозиторий_проектов / #project_repository
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetFilteredAsync(ProjectFilter filter)
        {
            // Build query with eager loading / Строим запрос с загрузкой связанных данных
            var query = _dbSet
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.Employee)
                .AsQueryable();

            // Apply date filters / Применяем фильтры по дате
            if (filter.StartDateFrom.HasValue)
                query = query.Where(p => p.StartDate >= filter.StartDateFrom.Value);

            if (filter.StartDateTo.HasValue)
                query = query.Where(p => p.StartDate <= filter.StartDateTo.Value);

            // Apply priority filter / Применяем фильтр по приоритету
            if (filter.Priority.HasValue)
                query = query.Where(p => p.Priority == filter.Priority.Value);

            // Apply sorting / Применяем сортировку
            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "startdate" => filter.SortDescending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                "enddate" => filter.SortDescending ? query.OrderByDescending(p => p.EndDate) : query.OrderBy(p => p.EndDate),
                "priority" => filter.SortDescending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
                _ => query.OrderBy(p => p.Name) // Default sorting / Сортировка по умолчанию
            };

            return await query.ToListAsync();
        }

        public async Task AddEmployeeAsync(Guid projectId, Guid employeeId)
        {
            // Only add if not already assigned / Добавляем только если ещё не назначен
            var alreadyExists = await IsEmployeeOnProjectAsync(projectId, employeeId);
            if (!alreadyExists)
            {
                _context.ProjectEmployees.Add(new ProjectEmployee
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveEmployeeAsync(Guid projectId, Guid employeeId)
        {
            var record = await _context.ProjectEmployees
                .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

            if (record is not null)
            {
                _context.ProjectEmployees.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsEmployeeOnProjectAsync(Guid projectId, Guid employeeId)
        {
            return await _context.ProjectEmployees
                .AnyAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);
        }
    }
}
