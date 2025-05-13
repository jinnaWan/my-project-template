using MyProject.Api.Data.Repositories;
using MyProject.Api.Models;
using Microsoft.Extensions.Logging;

namespace MyProject.Api.Services;

/// <summary>
/// Implementation of ITodoService that provides Todo business logic
/// </summary>
public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<TodoService> _logger;

    /// <summary>
    /// Initializes a new instance of the TodoService class
    /// </summary>
    /// <param name="todoRepository">The todo repository</param>
    /// <param name="logger">The logger instance</param>
    public TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger)
    {
        // some comments
        _todoRepository = todoRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Todo>> GetAllTodosAsync()
    {
        try
        {
            _logger.LogInformation("Getting all todos");
            return await _todoRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all todos");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Todo>> GetCompletedTodosAsync()
    {
        try
        {
            _logger.LogInformation("Getting completed todos");
            return await _todoRepository.GetCompletedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completed todos");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Todo>> GetIncompleteTodosAsync()
    {
        try
        {
            _logger.LogInformation("Getting incomplete todos");
            return await _todoRepository.GetIncompleteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting incomplete todos");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Todo?> GetTodoByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Getting todo with ID {Id}", id);
            var todo = await _todoRepository.GetByIdAsync(id);
            
            if (todo == null)
            {
                _logger.LogWarning("Todo with ID {Id} not found", id);
            }
            
            return todo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todo with ID {Id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Todo> CreateTodoAsync(Todo todo)
    {
        if (todo == null)
        {
            throw new ArgumentNullException(nameof(todo));
        }
        
        // Validation logic can be added here
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            _logger.LogWarning("Attempted to create todo with empty title");
            throw new ArgumentException("Todo title cannot be empty.", nameof(todo));
        }

        try
        {
            _logger.LogInformation("Creating new todo with title '{Title}'", todo.Title);
            return await _todoRepository.AddAsync(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo with title '{Title}'", todo.Title);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateTodoAsync(Todo todo)
    {
        if (todo == null)
        {
            throw new ArgumentNullException(nameof(todo));
        }
        
        // Validation logic can be added here
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            _logger.LogWarning("Attempted to update todo with empty title");
            throw new ArgumentException("Todo title cannot be empty.", nameof(todo));
        }

        try
        {
            _logger.LogInformation("Updating todo with ID {Id}", todo.Id);
            return await _todoRepository.UpdateAsync(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo with ID {Id}", todo.Id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTodoAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting todo with ID {Id}", id);
            var result = await _todoRepository.DeleteAsync(id);
            
            if (!result)
            {
                _logger.LogWarning("Todo with ID {Id} not found for deletion", id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo with ID {Id}", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> MarkAsCompletedAsync(int id)
    {
        try
        {
            _logger.LogInformation("Marking todo with ID {Id} as completed", id);
            var result = await _todoRepository.MarkAsCompletedAsync(id);
            
            if (!result)
            {
                _logger.LogWarning("Todo with ID {Id} not found for marking as completed", id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking todo with ID {Id} as completed", id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> MarkAsIncompleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Marking todo with ID {Id} as incomplete", id);
            var result = await _todoRepository.MarkAsIncompleteAsync(id);
            
            if (!result)
            {
                _logger.LogWarning("Todo with ID {Id} not found for marking as incomplete", id);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking todo with ID {Id} as incomplete", id);
            throw;
        }
    }
} 