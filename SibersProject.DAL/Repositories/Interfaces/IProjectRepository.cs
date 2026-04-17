using SibersProject.DAL.Entities;
using SibersProject.DAL.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Repositories.Interfaces
{
    // #тнтерфейс_репозитория_проектов / #project_repository_interface
    public interface IProjectRepository : IRepository<Project>
    {
        // Get project with all related date / Получить проект со всеми связаанными данными
        Task<Project?> GetWithDetailsAsync(Guid id);

        // Get filtered and sorted projects / Полусить отфильтрованный проекты
        Task<IEnumerable<Project>> GetFilteredAsync(ProjectFilter filter);

        // Add employee to project / Добавить сотрудника в проект
        Task AddEmployeeAsync(Guid projectId, Guid employeeId);

        // Remove emplyee from proejct / Убрать сотрудник из проекта
        Task RemoveEmployeeAsync(Guid projectId,Guid employeeId);

        // Check if employee is on project / Проверить состоит ли сотрудник в проекте
        Task<bool> IsEmployeeOnProjectAsync(Guid projectId, Guid employeeId);

        // Document metadata operations / Операции с метаданными документов
        Task AddDocumentsAsync(IEnumerable<ProjectDocument> documents);
        Task<ProjectDocument?> GetDocumentAsync(Guid projectId, Guid documentId);
        Task RemoveDocumentAsync(ProjectDocument document);
    }
}
