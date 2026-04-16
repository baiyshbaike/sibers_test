using Microsoft.EntityFrameworkCore;
using SibersProject.DAL.Data;
using SibersProject.DAL.Entities;
using SibersProject.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Repositories
{
    // #репозиторий_сотрудников / #employee_repository
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employee>> SearchAsync(string query)
        {
            // Case-insensitive partial match on FirstName, LastName, Email
            // Поиск без учёта регистра по имени, фамилии и email
            var lower = query.ToLower();

            return await _dbSet
                .Where(e =>
                    e.FirstName.ToLower().Contains(lower) ||
                    e.LastName.ToLower().Contains(lower) ||
                    (e.MiddleName != null && e.MiddleName.ToLower().Contains(lower)) ||
                    e.Email.ToLower().Contains(lower))
                .OrderBy(e => e.LastName)
                .Take(20) // Limit results for AJAX / Ограничение для AJAX
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
        {
            var query = _dbSet.Where(e => e.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
                query = query.Where(e => e.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
