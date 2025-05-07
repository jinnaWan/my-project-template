using MyProject.Api.Data;
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
    /// <param name="todoRepository">The Todo repository</param>
    public TodoService(ITodoRepository todoRepository)
    {
        // some comments
        _todoRepository = todoRepository;
    }

    /// <inheritdoc />
    public IEnumerable<Todo> GetAllTodos()
    {
        return _todoRepository.GetAll();
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
} 