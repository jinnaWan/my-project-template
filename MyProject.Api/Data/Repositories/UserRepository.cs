using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Api.Models;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Repository implementation for User entities
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the UserRepository class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <param name="logger">The logger instance</param>
        public UserRepository(ApplicationDbContext dbContext, ILogger<UserRepository> logger) : base(dbContext, logger)
        {
        }
        
        /// <inheritdoc />
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            _logger.LogInformation("Getting all users");
            return await _dbSet.OrderBy(u => u.Username).ToListAsync();
        }
        
        /// <inheritdoc />
        public override async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to delete user with ID {UserId}", id);
            
            var user = await _dbSet.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                return false;
            }
                
            _dbSet.Remove(user);
            var result = await _dbContext.SaveChangesAsync() > 0;
            _logger.LogInformation("User deletion result: {Result}", result);
            return result;
        }
        
        /// <inheritdoc />
        public async Task<User?> GetByUsernameAsync(string username)
        {
            _logger.LogInformation("Getting user by username: {Username}", username);
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }
        
        /// <inheritdoc />
        public async Task<User?> GetByEmailAsync(string email)
        {
            _logger.LogInformation("Getting user by email: {Email}", email);
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            _logger.LogInformation("Getting all active users");
            return await _dbSet.Where(u => u.IsActive).OrderBy(u => u.Username).ToListAsync();
        }
        
        /// <inheritdoc />
        public async Task<bool> ActivateUserAsync(int id)
        {
            _logger.LogInformation("Attempting to activate user with ID {UserId}", id);
            
            var user = await _dbSet.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for activation", id);
                return false;
            }
                
            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            
            var result = await _dbContext.SaveChangesAsync() > 0;
            _logger.LogInformation("User activation result: {Result}", result);
            return result;
        }
        
        /// <inheritdoc />
        public async Task<bool> DeactivateUserAsync(int id)
        {
            _logger.LogInformation("Attempting to deactivate user with ID {UserId}", id);
            
            var user = await _dbSet.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deactivation", id);
                return false;
            }
                
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            
            var result = await _dbContext.SaveChangesAsync() > 0;
            _logger.LogInformation("User deactivation result: {Result}", result);
            return result;
        }
        
        /// <inheritdoc />
        public override async Task<User> AddAsync(User user)
        {
            _logger.LogInformation("Adding new user: {Username}", user.Username);
            
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _dbSet.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
        
        /// <inheritdoc />
        public override async Task<bool> UpdateAsync(User user)
        {
            _logger.LogInformation("Updating user with ID {UserId}", user.Id);
            
            var existingUser = await _dbSet.FindAsync(user.Id);
            if (existingUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", user.Id);
                return false;
            }
                
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.IsActive = user.IsActive;
            existingUser.UpdatedAt = DateTime.UtcNow;
            
            var result = await _dbContext.SaveChangesAsync() > 0;
            _logger.LogInformation("User update result: {Result}", result);
            return result;
        }
    }
} 