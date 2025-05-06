using MyProject.Api.Models;

namespace MyProject.Api.Data;

/// <summary>
/// In-memory implementation of ITodoRepository
/// </summary>
public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<Todo> _todos;
    private int _nextId = 1;

    /// <summary>
    /// Initializes a new instance of the InMemoryTodoRepository class
    /// </summary>
    public InMemoryTodoRepository()
    {
        _todos = new List<Todo>
        {
            new Todo { Id = _nextId++, Title = "Learn ASP.NET Core", IsCompleted = true },
            new Todo { Id = _nextId++, Title = "Build a React application", IsCompleted = false },
            new Todo { Id = _nextId++, Title = "Connect API with frontend", IsCompleted = false }
        };
    }

    /// <inheritdoc />
    public IEnumerable<Todo> GetAll()
    {
        return _todos.ToList();
    }

    /// <inheritdoc />
    public Todo? GetById(int id)
    {
        return _todos.FirstOrDefault(t => t.Id == id);
    }

    /// <inheritdoc />
    public Todo Add(Todo todo)
    {
        todo.Id = _nextId++;
        _todos.Add(todo);
        return todo;
    }

    /// <inheritdoc />
    public bool Update(Todo todo)
    {
        var existingTodo = _todos.FirstOrDefault(t => t.Id == todo.Id);
        if (existingTodo == null)
        {
            return false;
        }

        existingTodo.Title = todo.Title;
        existingTodo.IsCompleted = todo.IsCompleted;
        return true;
    }

    /// <inheritdoc />
    public bool Delete(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
        {
            return false;
        }

        _todos.Remove(todo);
        return true;
    }
} 