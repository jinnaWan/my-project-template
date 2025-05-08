using MyProject.Api.Data.Repositories;

namespace MyProject.Api.Data
{
    /// <summary>
    /// Implementation of the unit of work pattern
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private ITodoRepository? _todoRepository;
        private IUserRepository? _userRepository;
        // Add additional repository fields here as needed
        // private IProductRepository _productRepository;
        
        /// <summary>
        /// Initializes a new instance of the UnitOfWork class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        /// <inheritdoc />
        public ITodoRepository Todos => _todoRepository ??= new TodoRepository(_dbContext);
        
        /// <inheritdoc />
        public IUserRepository Users => _userRepository ??= new UserRepository(_dbContext);
        
        // Implement additional repository properties here as needed
        // public IProductRepository Products => _productRepository ??= new ProductRepository(_dbContext);
        
        /// <inheritdoc />
        public int Complete()
        {
            return _dbContext.SaveChanges();
        }
        
        /// <summary>
        /// Disposes the database context
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
} 