using SibersProject.BLL.DTOs.Employee;
using SibersProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Mappings
{
    // #маппинг_сотрудника / #employee_mappings
    // Static extension methods for mapping between Employee entity and DTOs
    public static class EmployeeMappings
    {
        public static EmployeeDto ToDto(this Employee employee) => new()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            MiddleName = employee.MiddleName,
            Email = employee.Email
        };

        public static Employee ToEntity(this CreateEmployeeDto dto) => new()
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            Email = dto.Email
        };

        public static void ApplyUpdate(this Employee employee, UpdateEmployeeDto dto)
        {
            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.MiddleName = dto.MiddleName;
            employee.Email = dto.Email;
        }
    }
}
