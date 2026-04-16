using SibersProject.BLL.DTOs.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Services.Interfaces
{
    // #интерфейс_сервиса_сотрудников / #employee_service_interface
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<EmployeeDto>> SearchAsync(string query);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
        Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto dto);
        Task DeleteAsync(Guid id);
    }
}
