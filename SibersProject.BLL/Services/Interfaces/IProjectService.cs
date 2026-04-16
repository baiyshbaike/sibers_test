using SibersProject.BLL.DTOs.Project;
using SibersProject.DAL.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Services.Interfaces
{
    // #интерфейс_сервиса_проектов / #project_service_interface
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync(ProjectFilter filter);
        Task<ProjectDto?> GetByIdAsync(Guid id);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto);
        Task DeleteAsync(Guid id);
        Task AddEmployeeAsync(Guid projectId, Guid employeeId);
        Task RemoveEmployeeAsync(Guid projectId, Guid employeeId);
    }
}
