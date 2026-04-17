using SibersProject.BLL.DTOs.Project;
using SibersProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Mappings
{
    // #маппинг_проекта / #project_mappings
    public static class ProjectMappings
    {
        public static ProjectDto ToDto(this Project project) => new()
        {
            Id = project.Id,
            Name = project.Name,
            CustomerCompany = project.CustomerCompany,
            ExecutorCompany = project.ExecutorCompany,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Priority = project.Priority,
            ProjectManager = project.ProjectManager?.ToDto(),
            Employees = project.ProjectEmployees?
                .Select(pe => pe.Employee.ToDto())
                .ToList() ?? new(),
            Documents = project.Documents?
                .Select(d => new ProjectDocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    ContentType = d.ContentType,
                    Size = d.Size,
                    UploadedAtUtc = d.UploadedAtUtc
                })
                .ToList() ?? new()
        };

        public static Project ToEntity(this CreateProjectDto dto) => new()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            CustomerCompany = dto.CustomerCompany,
            ExecutorCompany = dto.ExecutorCompany,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Priority = dto.Priority,
            ProjectManagerId = dto.ProjectManagerId
        };

        public static void ApplyUpdate(this Project project, UpdateProjectDto dto)
        {
            project.Name = dto.Name;
            project.CustomerCompany = dto.CustomerCompany;
            project.ExecutorCompany = dto.ExecutorCompany;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.Priority = dto.Priority;
            project.ProjectManagerId = dto.ProjectManagerId;
        }
    }
}
