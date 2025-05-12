using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Base implementation of the generic repository pattern
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public abstract class BaseRepository<T> : StoredProcedureRepository, IRepository<T> where T : class
    {
        // Using new keyword to explicitly hide the base class field
        protected new readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;
        protected new readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the BaseRepository class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <param name="logger">The logger</param>
        protected BaseRepository(ApplicationDbContext dbContext, ILogger logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
            _logger = logger;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <inheritdoc />
        public abstract Task<bool> DeleteAsync(int id);
    }
} 