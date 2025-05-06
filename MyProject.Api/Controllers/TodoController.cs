using Microsoft.AspNetCore.Mvc;
using MyProject.Api.Models;
using MyProject.Api.Services;

namespace MyProject.Api.Controllers;

/// <summary>
/// API controller for managing Todo items
/// </summary>
[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoController> _logger;

    /// <summary>
    /// Initializes a new instance of the TodoController class
    /// </summary>
    /// <param name="todoService">The Todo service</param>
    /// <param name="logger">The logger</param>
    public TodoController(ITodoService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all Todo items
    /// </summary>
    /// <returns>A collection of Todo items</returns>
    [HttpGet]
    public ActionResult<IEnumerable<Todo>> GetAll()
    {
        try
        {
            var todos = _todoService.GetAllTodos();
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all todos");
            return StatusCode(500, "An error occurred while retrieving todos");
        }
    }

    /// <summary>
    /// Gets a Todo item by its identifier
    /// </summary>
    /// <param name="id">The Todo item identifier</param>
    /// <returns>The Todo item if found</returns>
    [HttpGet("{id}")]
    public ActionResult<Todo> GetById(int id)
    {
        try
        {
            var todo = _todoService.GetTodoById(id);
            if (todo == null)
            {
                return NotFound($"Todo with ID {id} not found");
            }

            return Ok(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todo with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the todo");
        }
    }

    /// <summary>
    /// Creates a new Todo item
    /// </summary>
    /// <param name="todo">The Todo item to create</param>
    /// <returns>The created Todo item</returns>
    [HttpPost]
    public ActionResult<Todo> Create(Todo todo)
    {
        try
        {
            var createdTodo = _todoService.CreateTodo(todo);
            return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo");
            return StatusCode(500, "An error occurred while creating the todo");
        }
    }

    /// <summary>
    /// Updates an existing Todo item
    /// </summary>
    /// <param name="id">The identifier of the Todo item to update</param>
    /// <param name="todo">The updated Todo item</param>
    /// <returns>No content if successful</returns>
    [HttpPut("{id}")]
    public IActionResult Update(int id, Todo todo)
    {
        if (id != todo.Id)
        {
            return BadRequest("ID in the URL does not match the ID in the request body");
        }

        try
        {
            var success = _todoService.UpdateTodo(todo);
            if (!success)
            {
                return NotFound($"Todo with ID {id} not found");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the todo");
        }
    }

    /// <summary>
    /// Deletes a Todo item
    /// </summary>
    /// <param name="id">The identifier of the Todo item to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var success = _todoService.DeleteTodo(id);
            if (!success)
            {
                return NotFound($"Todo with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the todo");
        }
    }
} 