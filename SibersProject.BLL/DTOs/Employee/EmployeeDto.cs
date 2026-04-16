using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.DTOs.Employee
{
    // #DTO_сотрудника / #employee_dto
    // Data transfer object for reading employee data
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = string.Empty;

        // Full name helper / Полное имя
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}
