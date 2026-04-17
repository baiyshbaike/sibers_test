using Microsoft.EntityFrameworkCore;
using SibersProject.DAL.Data;
using SibersProject.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SibersProject.DAL.Repositories
{
    // #базовы_репозиторий / #generic_repository
    // Generic base repository with common CRUD operations
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();
        public virtual async Task<T?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);
        public virtual async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is not null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
