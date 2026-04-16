using SibersProject.BLL.DTOs.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.DTOs.Project
{
    // #DTO_проекта / #project_dto
    // Full project DTO including manager and employees
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CustomerCompany { get; set; } = string.Empty;
        public string ExecutorCompany { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public EmployeeDto? ProjectManager { get; set; }
        public List<EmployeeDto> Employees { get; set; } = new();
    }
}
