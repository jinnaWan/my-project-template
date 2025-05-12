using MyProject.Api.Models;

namespace MyProject.Api.Services;

/// <summary>
/// Defines operations for Todo business logic
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// Gets all Todo items asynchronously
    /// </summary>
    /// <returns>A collection of Todo items</returns>
    Task<IEnumerable<Todo>> GetAllTodosAsync();
    
    /// <summary>
    /// Gets all completed Todo items asynchronously
    /// </summary>
    /// <returns>A collection of completed Todo items</returns>
    Task<IEnumerable<Todo>> GetCompletedTodosAsync();
    
    /// <summary>
    /// Gets all incomplete Todo items asynchronously
    /// </summary>
    /// <returns>A collection of incomplete Todo items</returns>
    Task<IEnumerable<Todo>> GetIncompleteTodosAsync();
    
    /// <summary>
    /// Gets a Todo item by its identifier asynchronously
    /// </summary>
    /// <param name="id">The Todo item identifier</param>
    /// <returns>The Todo item if found; otherwise null</returns>
    Task<Todo?> GetTodoByIdAsync(int id);
    
    /// <summary>
    /// Creates a new Todo item asynchronously
    /// </summary>
    /// <param name="todo">The Todo item to create</param>
    /// <returns>The created Todo item with its assigned identifier</returns>
    Task<Todo> CreateTodoAsync(Todo todo);
    
    /// <summary>
    /// Updates an existing Todo item asynchronously
    /// </summary>
    /// <param name="todo">The Todo item to update</param>
    /// <returns>True if the update was successful; otherwise false</returns>
    Task<bool> UpdateTodoAsync(Todo todo);
    
    /// <summary>
    /// Deletes a Todo item by its identifier asynchronously
    /// </summary>
    /// <param name="id">The identifier of the Todo item to delete</param>
    /// <returns>True if the deletion was successful; otherwise false</returns>
    Task<bool> DeleteTodoAsync(int id);
    
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
} 