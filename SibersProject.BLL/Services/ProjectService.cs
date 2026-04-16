using SibersProject.BLL.DTOs.Project;
using SibersProject.BLL.Services.Interfaces;
using SibersProject.DAL.Filters;
using SibersProject.DAL.Repositories.Interfaces;
using SibersProject.BLL.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Services
{
    // #сервис_проектов / #project_service
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ProjectService(
            IProjectRepository projectRepository,
            IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync(ProjectFilter filter)
        {
            var projects = await _projectRepository.GetFilteredAsync(filter);
            return projects.Select(p => p.ToDto());
        }

        public async Task<ProjectDto?> GetByIdAsync(Guid id)
        {
            var project = await _projectRepository.GetWithDetailsAsync(id);
            return project?.ToDto();
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            // Validate manager exists / Проверяем существование руководителя
            var manager = await _employeeRepository.GetByIdAsync(dto.ProjectManagerId)
                ?? throw new KeyNotFoundException($"Employee (manager) with id '{dto.ProjectManagerId}' not found.");

            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("Start date cannot be later than end date.");

            var entity = dto.ToEntity();
            var created = await _projectRepository.CreateAsync(entity);

            // Return with full details / Возвращаем с полными данными
            var detailed = await _projectRepository.GetWithDetailsAsync(created.Id);
            return detailed!.ToDto();
        }

        public async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto)
        {
            var project = await _projectRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Project with id '{id}' not found.");

            var manager = await _employeeRepository.GetByIdAsync(dto.ProjectManagerId)
                ?? throw new KeyNotFoundException($"Employee (manager) with id '{dto.ProjectManagerId}' not found.");

            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("Start date cannot be later than end date.");

            project.ApplyUpdate(dto);
            await _projectRepository.UpdateAsync(project);

            var detailed = await _projectRepository.GetWithDetailsAsync(id);
            return detailed!.ToDto();
        }

        public async Task DeleteAsync(Guid id)
        {
            _ = await _projectRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Project with id '{id}' not found.");

            await _projectRepository.DeleteAsync(id);
        }

        public async Task AddEmployeeAsync(Guid projectId, Guid employeeId)
        {
            _ = await _projectRepository.GetByIdAsync(projectId)
                ?? throw new KeyNotFoundException($"Project with id '{projectId}' not found.");

            _ = await _employeeRepository.GetByIdAsync(employeeId)
                ?? throw new KeyNotFoundException($"Employee with id '{employeeId}' not found.");

            await _projectRepository.AddEmployeeAsync(projectId, employeeId);
        }

        public async Task RemoveEmployeeAsync(Guid projectId, Guid employeeId)
        {
            _ = await _projectRepository.GetByIdAsync(projectId)
                ?? throw new KeyNotFoundException($"Project with id '{projectId}' not found.");

            await _projectRepository.RemoveEmployeeAsync(projectId, employeeId);
        }
    }
}
