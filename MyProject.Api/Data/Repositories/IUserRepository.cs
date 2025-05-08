using MyProject.Api.Models;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Repository interface for User entities
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Gets a user by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user if found; otherwise null</returns>
        User? GetByUsername(string username);
        
        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">The email address</param>
        /// <returns>The user if found; otherwise null</returns>
        User? GetByEmail(string email);
        
        /// <summary>
        /// Gets all active users
        /// </summary>
        /// <returns>A collection of active users</returns>
        IEnumerable<User> GetActiveUsers();
        
        /// <summary>
        /// Activates a user
        /// </summary>
        /// <param name="id">The user identifier</param>
        /// <returns>True if the operation was successful; otherwise false</returns>
        bool ActivateUser(int id);
        
        /// <summary>
        /// Deactivates a user
        /// </summary>
        /// <param name="id">The user identifier</param>
        /// <returns>True if the operation was successful; otherwise false</returns>
        bool DeactivateUser(int id);
    }
} 