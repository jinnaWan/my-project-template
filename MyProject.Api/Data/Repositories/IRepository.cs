using System.Linq.Expressions;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities asynchronously
        /// </summary>
        /// <returns>A collection of entities</returns>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// Gets entities based on a filter asynchronously
        /// </summary>
        /// <param name="filter">The filter expression</param>
        /// <returns>A filtered collection of entities</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
        
        /// <summary>
        /// Gets an entity by its identifier asynchronously
        /// </summary>
        /// <param name="id">The entity identifier</param>
        /// <returns>The entity if found; otherwise null</returns>
        Task<T?> GetByIdAsync(int id);
        
        /// <summary>
        /// Adds a new entity asynchronously
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity</returns>
        Task<T> AddAsync(T entity);
        
        /// <summary>
        /// Updates an existing entity asynchronously
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <returns>True if the update was successful; otherwise false</returns>
        Task<bool> UpdateAsync(T entity);
        
        /// <summary>
        /// Deletes an entity by its identifier asynchronously
        /// </summary>
        /// <param name="id">The identifier of the entity to delete</param>
        /// <returns>True if the deletion was successful; otherwise false</returns>
        Task<bool> DeleteAsync(int id);
        
        // Keep synchronous methods for backward compatibility
        
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>A collection of entities</returns>
        IEnumerable<T> GetAll();
        
        /// <summary>
        /// Gets entities based on a filter
        /// </summary>
        /// <param name="filter">The filter expression</param>
        /// <returns>A filtered collection of entities</returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);
        
        /// <summary>
        /// Gets an entity by its identifier
        /// </summary>
        /// <param name="id">The entity identifier</param>
        /// <returns>The entity if found; otherwise null</returns>
        T? GetById(int id);
        
        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity</returns>
        T Add(T entity);
        
        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <returns>True if the update was successful; otherwise false</returns>
        bool Update(T entity);
        
        /// <summary>
        /// Deletes an entity by its identifier
        /// </summary>
        /// <param name="id">The identifier of the entity to delete</param>
        /// <returns>True if the deletion was successful; otherwise false</returns>
        bool Delete(int id);
    }
} 