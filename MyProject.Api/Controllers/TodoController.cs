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
    [HttpGet(Name = "GetAllTodos")]
    public async Task<ActionResult<IEnumerable<Todo>>> GetAllAsync()
    {
        try
        {
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetAllAsync");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Gets a Todo item by its identifier
    /// </summary>
    /// <param name="id">The Todo item identifier</param>
    /// <returns>The Todo item if found</returns>
    [HttpGet("{id}", Name = "GetTodoById")]
    public async Task<ActionResult<Todo>> GetByIdAsync(int id)
    {
        try
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetByIdAsync");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Creates a new Todo item
    /// </summary>
    /// <param name="todo">The Todo item to create</param>
    /// <returns>The created Todo item</returns>
    [HttpPost(Name = "CreateTodo")]
    public async Task<ActionResult<Todo>> CreateAsync(Todo todo)
    {
        try
        {
            var createdTodo = await _todoService.CreateTodoAsync(todo);
            return CreatedAtRoute("GetTodoById", new { id = createdTodo.Id }, createdTodo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CreateAsync");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Updates an existing Todo item
    /// </summary>
    /// <param name="id">The identifier of the Todo item to update</param>
    /// <param name="todo">The updated Todo item</param>
    /// <returns>No content if successful</returns>
    [HttpPut("{id}", Name = "UpdateTodo")]
    public async Task<IActionResult> UpdateAsync(int id, Todo todo)
    {
        if (id != todo.Id)
        {
            return BadRequest("ID in the URL does not match the ID in the request body");
        }

        try
        {
            var success = await _todoService.UpdateTodoAsync(todo);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in UpdateAsync");
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    /// <summary>
    /// Deletes a Todo item
    /// </summary>
    /// <param name="id">The identifier of the Todo item to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}", Name = "DeleteTodo")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var success = await _todoService.DeleteTodoAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in DeleteAsync");
            return StatusCode(500, "An unexpected error occurred");
        }
    }
} 