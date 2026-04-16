using SibersProject.BLL.DTOs.Employee;
using SibersProject.BLL.Services.Interfaces;
using SibersProject.DAL.Repositories.Interfaces;
using SibersProject.BLL.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.BLL.Services
{
    // #сервис_сотрудников / #employee_service
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(e => e.ToDto());
        }

        public async Task<EmployeeDto?> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee?.ToDto();
        }

        public async Task<IEnumerable<EmployeeDto>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<EmployeeDto>();

            var employees = await _employeeRepository.SearchAsync(query);
            return employees.Select(e => e.ToDto());
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
        {
            // Check email uniqueness / Проверяем уникальность email
            var emailExists = await _employeeRepository.EmailExistsAsync(dto.Email);
            if (emailExists)
                throw new InvalidOperationException($"Employee with email '{dto.Email}' already exists.");

            var entity = dto.ToEntity();
            var created = await _employeeRepository.CreateAsync(entity);
            return created.ToDto();
        }

        public async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee with id '{id}' not found.");

            // Check email uniqueness excluding current employee
            // Проверяем уникальность email, исключая текущего сотрудника
            var emailExists = await _employeeRepository.EmailExistsAsync(dto.Email, id);
            if (emailExists)
                throw new InvalidOperationException($"Email '{dto.Email}' is already taken.");

            employee.ApplyUpdate(dto);
            var updated = await _employeeRepository.UpdateAsync(employee);
            return updated.ToDto();
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee with id '{id}' not found.");

            await _employeeRepository.DeleteAsync(id);
        }
    }
}
