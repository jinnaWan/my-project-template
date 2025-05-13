using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Api.Data;
using MyProject.Api.Data.Repositories;
using MyProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProject.Api.UnitTests.Data
{
    /// <summary>
    /// Testing implementation of TodoRepository that doesn't use stored procedures
    /// </summary>
    public class TestTodoRepository : ITodoRepository
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<Todo> _dbSet;
        protected readonly ILogger _logger;

        public TestTodoRepository(ApplicationDbContext dbContext, ILogger<TestTodoRepository> logger)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Todos;
            _logger = logger;
        }

        public virtual async Task<Todo> AddAsync(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            
            await _dbSet.AddAsync(todo);
            await _dbContext.SaveChangesAsync();
            return todo;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var todo = await _dbSet.FindAsync(id);
            if (todo == null)
            {
                _logger.LogWarning("Todo with ID {TodoId} not found for deletion", id);
                return false;
            }
                
            _dbSet.Remove(todo);
            var result = await _dbContext.SaveChangesAsync() > 0;
            _logger.LogInformation("Todo deletion result: {Result}", result);
            return result;
        }

        public virtual async Task<IEnumerable<Todo>> GetAllAsync()
        {
            return await _dbSet.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Todo>> GetCompletedAsync()
        {
            return await _dbSet.Where(t => t.IsCompleted).OrderByDescending(t => t.UpdatedAt).ToListAsync();
        }

        public virtual async Task<Todo?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Todo>> GetIncompleteAsync()
        {
            return await _dbSet.Where(t => !t.IsCompleted).OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<bool> MarkAsCompletedAsync(int id)
        {
            var todo = await _dbSet.FindAsync(id);
            if (todo == null)
                return false;
                
            todo.IsCompleted = true;
            todo.UpdatedAt = DateTime.UtcNow;
            
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkAsIncompleteAsync(int id)
        {
            var todo = await _dbSet.FindAsync(id);
            if (todo == null)
                return false;
                
            todo.IsCompleted = false;
            todo.UpdatedAt = DateTime.UtcNow;
            
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> UpdateAsync(Todo todo)
        {
            var existingTodo = await _dbSet.FindAsync(todo.Id);
            if (existingTodo == null)
            {
                _logger.LogWarning("Todo with ID {TodoId} not found for update", todo.Id);
                return false;
            }
                
            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.IsCompleted = todo.IsCompleted;
            existingTodo.UpdatedAt = DateTime.UtcNow;
            
            var result = await _dbContext.SaveChangesAsync() > 0;
            _logger.LogInformation("Todo update result: {Result}", result);
            return result;
        }

        public async Task<IEnumerable<Todo>> FindAsync(Expression<Func<Todo, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }
    }
} 