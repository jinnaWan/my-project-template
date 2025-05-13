using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Api.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace MyProject.Api.UnitTests.Data
{
    /// <summary>
    /// Base repository for tests that uses TestDbContext
    /// </summary>
    public class TestRepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly TestDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger _logger;

        public TestRepositoryBase(TestDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
            _logger = logger;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Entity with ID {Id} not found for deletion", id);
                return false;
            }

            _dbSet.Remove(entity);
            var result = await _dbContext.SaveChangesAsync() > 0;
            _logger.LogInformation("Entity deletion result: {Result}", result);
            return result;
        }

        public virtual async Task<System.Collections.Generic.IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }

        public virtual async Task<System.Collections.Generic.IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
} 