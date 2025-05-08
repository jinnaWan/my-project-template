using MyProject.Api.Data.Repositories;
using MyProject.Api.Models;

namespace MyProject.Api.Services;

/// <summary>
/// Implementation of ITodoService that provides Todo business logic
/// </summary>
public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;

    /// <summary>
    /// Initializes a new instance of the TodoService class
    /// </summary>
    /// <param name="todoRepository">The todo repository</param>
    public TodoService(ITodoRepository todoRepository)
    {
        // some comments
        _todoRepository = todoRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Todo>> GetAllTodosAsync()
    {
        return await _todoRepository.GetAllAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Todo>> GetCompletedTodosAsync()
    {
        return await _todoRepository.GetCompletedAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Todo>> GetIncompleteTodosAsync()
    {
        return await _todoRepository.GetIncompleteAsync();
    }

    /// <inheritdoc />
    public async Task<Todo?> GetTodoByIdAsync(int id)
    {
        return await _todoRepository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task<Todo> CreateTodoAsync(Todo todo)
    {
        // Validation logic can be added here
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new ArgumentException("Todo title cannot be empty.", nameof(todo));
        }

        return await _todoRepository.AddAsync(todo);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateTodoAsync(Todo todo)
    {
        // Validation logic can be added here
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new ArgumentException("Todo title cannot be empty.", nameof(todo));
        }

        return await _todoRepository.UpdateAsync(todo);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTodoAsync(int id)
    {
        return await _todoRepository.DeleteAsync(id);
    }

    /// <inheritdoc />
    public async Task<bool> MarkAsCompletedAsync(int id)
    {
        return await _todoRepository.MarkAsCompletedAsync(id);
    }

    /// <inheritdoc />
    public async Task<bool> MarkAsIncompleteAsync(int id)
    {
        return await _todoRepository.MarkAsIncompleteAsync(id);
    }

    /// <inheritdoc />
    public IEnumerable<Todo> GetAllTodos()
    {
        return _todoRepository.GetAll();
    }

    /// <summary>
    /// Gets all completed Todo items
    /// </summary>
    /// <returns>A collection of completed Todo items</returns>
    public IEnumerable<Todo> GetCompletedTodos()
    {
        return _todoRepository.GetCompleted();
    }

    /// <summary>
    /// Gets all incomplete Todo items
    /// </summary>
    /// <returns>A collection of incomplete Todo items</returns>
    public IEnumerable<Todo> GetIncompleteTodos()
    {
        return _todoRepository.GetIncomplete();
    }

    /// <inheritdoc />
    public Todo? GetTodoById(int id)
    {
        return _todoRepository.GetById(id);
    }

    /// <inheritdoc />
    public Todo CreateTodo(Todo todo)
    {
        // Validation logic can be added here
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new ArgumentException("Todo title cannot be empty.", nameof(todo));
        }

        return _todoRepository.Add(todo);
    }

    /// <inheritdoc />
    public bool UpdateTodo(Todo todo)
    {
        // Validation logic can be added here
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new ArgumentException("Todo title cannot be empty.", nameof(todo));
        }

        return _todoRepository.Update(todo);
    }

    /// <inheritdoc />
    public bool DeleteTodo(int id)
    {
        return _todoRepository.Delete(id);
    }

    /// <summary>
    /// Marks a Todo item as completed
    /// </summary>
    /// <param name="id">The Todo item identifier</param>
    /// <returns>True if the operation was successful; otherwise false</returns>
    public bool MarkAsCompleted(int id)
    {
        return _todoRepository.MarkAsCompleted(id);
    }

    /// <summary>
    /// Marks a Todo item as incomplete
    /// </summary>
    /// <param name="id">The Todo item identifier</param>
    /// <returns>True if the operation was successful; otherwise false</returns>
    public bool MarkAsIncomplete(int id)
    {
        return _todoRepository.MarkAsIncomplete(id);
    }
} 