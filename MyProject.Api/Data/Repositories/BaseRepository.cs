using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Initializes a new instance of the BaseRepository class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        protected BaseRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
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

        /// <inheritdoc />
        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        /// <inheritdoc />
        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(filter).ToList();
        }

        /// <inheritdoc />
        public virtual T? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        /// <inheritdoc />
        public virtual T Add(T entity)
        {
            _dbSet.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        /// <inheritdoc />
        public virtual bool Update(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return _dbContext.SaveChanges() > 0;
        }

        /// <inheritdoc />
        public abstract bool Delete(int id);
    }
} 