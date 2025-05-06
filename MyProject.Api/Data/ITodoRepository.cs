using MyProject.Api.Models;

namespace MyProject.Api.Data;

/// <summary>
/// Defines operations for managing Todo items
/// </summary>
public interface ITodoRepository
{
    /// <summary>
    /// Gets all Todo items
    /// </summary>
    /// <returns>A collection of Todo items</returns>
    IEnumerable<Todo> GetAll();
    
    /// <summary>
    /// Gets a Todo item by its identifier
    /// </summary>
    /// <param name="id">The Todo item identifier</param>
    /// <returns>The Todo item if found; otherwise null</returns>
    Todo? GetById(int id);
    
    /// <summary>
    /// Adds a new Todo item
    /// </summary>
    /// <param name="todo">The Todo item to add</param>
    /// <returns>The added Todo item with its assigned identifier</returns>
    Todo Add(Todo todo);
    
    /// <summary>
    /// Updates an existing Todo item
    /// </summary>
    /// <param name="todo">The Todo item to update</param>
    /// <returns>True if the update was successful; otherwise false</returns>
    bool Update(Todo todo);
    
    /// <summary>
    /// Deletes a Todo item by its identifier
    /// </summary>
    /// <param name="id">The identifier of the Todo item to delete</param>
    /// <returns>True if the deletion was successful; otherwise false</returns>
    bool Delete(int id);
} 