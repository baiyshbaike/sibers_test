using SibersProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Repositories.Interfaces
{
    // #интерфейс_репозитория_сотрудников / #employee_repository_interface
    public interface IEmployeeRepository : IRepository<Employee>
    {
        // Search by partial name or email for AJAX autocomplete
        // Поиск по части имени или email для AJAX автодополнения
        Task<IEnumerable<Employee>> SearchAsync(string query);
        
        //Check if email alredy exists / Проверка уникальнисти email
        Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    }
}
