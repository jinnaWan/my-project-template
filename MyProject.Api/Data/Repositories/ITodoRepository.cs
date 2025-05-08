using MyProject.Api.Models;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Repository interface for Todo entities
    /// </summary>
    public interface ITodoRepository : IRepository<Todo>
    {
        /// <summary>
        /// Gets all completed Todo items asynchronously
        /// </summary>
        /// <returns>A collection of completed Todo items</returns>
        Task<IEnumerable<Todo>> GetCompletedAsync();
        
        /// <summary>
        /// Gets all incomplete Todo items asynchronously
        /// </summary>
        /// <returns>A collection of incomplete Todo items</returns>
        Task<IEnumerable<Todo>> GetIncompleteAsync();
        
        /// <summary>
        /// Marks a Todo item as completed asynchronously
        /// </summary>
        /// <param name="id">The Todo item identifier</param>
        /// <returns>True if the operation was successful; otherwise false</returns>
        Task<bool> MarkAsCompletedAsync(int id);
        
        /// <summary>
        /// Marks a Todo item as incomplete asynchronously
        /// </summary>
        /// <param name="id">The Todo item identifier</param>
        /// <returns>True if the operation was successful; otherwise false</returns>
        Task<bool> MarkAsIncompleteAsync(int id);
        
        /// <summary>
        /// Gets all completed Todo items
        /// </summary>
        /// <returns>A collection of completed Todo items</returns>
        IEnumerable<Todo> GetCompleted();
        
        /// <summary>
        /// Gets all incomplete Todo items
        /// </summary>
        /// <returns>A collection of incomplete Todo items</returns>
        IEnumerable<Todo> GetIncomplete();
        
        /// <summary>
        /// Marks a Todo item as completed
        /// </summary>
        /// <param name="id">The Todo item identifier</param>
        /// <returns>True if the operation was successful; otherwise false</returns>
        bool MarkAsCompleted(int id);
        
        /// <summary>
        /// Marks a Todo item as incomplete
        /// </summary>
        /// <param name="id">The Todo item identifier</param>
        /// <returns>True if the operation was successful; otherwise false</returns>
        bool MarkAsIncomplete(int id);
    }
} 