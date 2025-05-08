using MyProject.Api.Data.Repositories;

namespace MyProject.Api.Data
{
    /// <summary>
    /// Manages multiple repositories as a unit
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repository for Todo entities
        /// </summary>
        ITodoRepository Todos { get; }
        
        /// <summary>
        /// Repository for User entities
        /// </summary>
        IUserRepository Users { get; }
        
        // Add additional repositories here as needed
        // IProductRepository Products { get; }
        
        /// <summary>
        /// Saves all changes made in this unit of work to the database
        /// </summary>
        /// <returns>The number of state entries written to the database</returns>
        int Complete();
    }
} 