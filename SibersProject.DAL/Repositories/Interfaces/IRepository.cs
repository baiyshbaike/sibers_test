using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Repositories.Interfaces
{
    // #интерфейс_репозитория / #generic_repository_interface
    // Base CRUD interface for all repositories
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}
