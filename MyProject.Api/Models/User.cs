namespace MyProject.Api.Models
{
    /// <summary>
    /// Represents a user in the system
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the email address
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        public string FirstName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        public string LastName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user is active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
} 