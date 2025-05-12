using MyProject.Api.Models;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Repository interface for User entities
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Gets a user by username asynchronously
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user if found; otherwise null</returns>
        Task<User?> GetByUsernameAsync(string username);
        
        /// <summary>
        /// Gets a user by email asynchronously
        /// </summary>
        /// <param name="email">The email address</param>
        /// <returns>The user if found; otherwise null</returns>
        Task<User?> GetByEmailAsync(string email);
        
        /// <summary>
        /// Gets all active users asynchronously
        /// </summary>
        /// <returns>A collection of active users</returns>
        Task<IEnumerable<User>> GetActiveUsersAsync();
        
        /// <summary>
        /// Activates a user asynchronously
        /// </summary>
        /// <param name="id">The user identifier</param>
        /// <returns>True if the operation was successful; otherwise false</returns>
        Task<bool> ActivateUserAsync(int id);
        
        /// <summary>
        /// Deactivates a user asynchronously
        /// </summary>
        /// <param name="id">The user identifier</param>
        /// <returns>True if the operation was successful; otherwise false</returns>
        Task<bool> DeactivateUserAsync(int id);
    }
} 