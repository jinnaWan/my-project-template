using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Api.Data;
using MyProject.Api.Data.Repositories;
using MyProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProject.Api.UnitTests.Data
{
    /// <summary>
    /// Testing implementation of UserRepository that doesn't use stored procedures
    /// </summary>
    public class TestUserRepository : IUserRepository
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<User> _dbSet;
        protected readonly ILogger _logger;

        public TestUserRepository(ApplicationDbContext dbContext, ILogger<TestUserRepository> logger)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Users;
            _logger = logger;
        }

        public virtual async Task<User> AddAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _dbSet.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
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

        public virtual async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet.OrderBy(u => u.Username).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet.Where(u => u.IsActive).OrderBy(u => u.Username).ToListAsync();
        }

        public virtual async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
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

        public async Task<bool> DeactivateUserAsync(int id)
        {
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

        public virtual async Task<bool> UpdateAsync(User user)
        {
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

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }
    }
} 