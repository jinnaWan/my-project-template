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
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        
        /// <inheritdoc />
        public override IEnumerable<User> GetAll()
        {
            return _dbSet.OrderBy(u => u.Username).ToList();
        }
        
        /// <inheritdoc />
        public override bool Delete(int id)
        {
            var user = _dbSet.Find(id);
            if (user == null)
                return false;
                
            _dbSet.Remove(user);
            return _dbContext.SaveChanges() > 0;
        }
        
        /// <inheritdoc />
        public override async Task<bool> DeleteAsync(int id)
        {
            var user = await _dbSet.FindAsync(id);
            if (user == null)
                return false;
                
            _dbSet.Remove(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        
        /// <inheritdoc />
        public User? GetByUsername(string username)
        {
            return _dbSet.FirstOrDefault(u => u.Username == username);
        }
        
        /// <inheritdoc />
        public User? GetByEmail(string email)
        {
            return _dbSet.FirstOrDefault(u => u.Email == email);
        }
        
        /// <inheritdoc />
        public IEnumerable<User> GetActiveUsers()
        {
            return _dbSet.Where(u => u.IsActive).OrderBy(u => u.Username).ToList();
        }
        
        /// <inheritdoc />
        public bool ActivateUser(int id)
        {
            var user = _dbSet.Find(id);
            if (user == null)
                return false;
                
            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            
            return _dbContext.SaveChanges() > 0;
        }
        
        /// <inheritdoc />
        public bool DeactivateUser(int id)
        {
            var user = _dbSet.Find(id);
            if (user == null)
                return false;
                
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            
            return _dbContext.SaveChanges() > 0;
        }
        
        /// <inheritdoc />
        public override User Add(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            return base.Add(user);
        }
        
        /// <inheritdoc />
        public override bool Update(User user)
        {
            var existingUser = _dbSet.Find(user.Id);
            if (existingUser == null)
                return false;
                
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.IsActive = user.IsActive;
            existingUser.UpdatedAt = DateTime.UtcNow;
            
            return _dbContext.SaveChanges() > 0;
        }
    }
} 