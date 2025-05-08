namespace MyProject.Api.Models;

/// <summary>
/// Represents a Todo item in the application
/// </summary>
public class Todo
{
    /// <summary>
    /// Gets or sets the unique identifier for the Todo item
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the title of the Todo item
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the Todo item
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the Todo item is completed
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Gets or sets the creation date of the Todo item
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the last update date of the Todo item
    /// </summary>
    public DateTime UpdatedAt { get; set; }
} 