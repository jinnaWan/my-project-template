using MyProject.Api.Models;

namespace MyProject.Api.Services;

/// <summary>
/// Defines operations for Todo business logic
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// Gets all Todo items
    /// </summary>
    /// <returns>A collection of Todo items</returns>
    IEnumerable<Todo> GetAllTodos();
    
    /// <summary>
    /// Gets a Todo item by its identifier
    /// </summary>
    /// <param name="id">The Todo item identifier</param>
    /// <returns>The Todo item if found; otherwise null</returns>
    Todo? GetTodoById(int id);
    
    /// <summary>
    /// Creates a new Todo item
    /// </summary>
    /// <param name="todo">The Todo item to create</param>
    /// <returns>The created Todo item with its assigned identifier</returns>
    Todo CreateTodo(Todo todo);
    
    /// <summary>
    /// Updates an existing Todo item
    /// </summary>
    /// <param name="todo">The Todo item to update</param>
    /// <returns>True if the update was successful; otherwise false</returns>
    bool UpdateTodo(Todo todo);
    
    /// <summary>
    /// Deletes a Todo item by its identifier
    /// </summary>
    /// <param name="id">The identifier of the Todo item to delete</param>
    /// <returns>True if the deletion was successful; otherwise false</returns>
    bool DeleteTodo(int id);
} 