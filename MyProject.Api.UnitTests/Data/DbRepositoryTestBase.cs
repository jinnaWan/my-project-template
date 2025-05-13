using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Data.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Data
{
    /// <summary>
    /// Base class for repository tests using real DbContext
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TRepository">The repository interface type</typeparam>
    /// <typeparam name="TRepositoryImpl">The repository implementation type</typeparam>
    /// <typeparam name="TContext">The DbContext type</typeparam>
    public abstract class DbRepositoryTestBase<TEntity, TRepository, TRepositoryImpl, TContext> 
        : RepositoryTestBase<TEntity, TRepository>
        where TEntity : class
        where TRepository : class, IRepository<TEntity>
        where TRepositoryImpl : class, TRepository
        where TContext : DbContext
    {
        protected readonly Mock<ILogger<TRepositoryImpl>> _mockLogger;
        protected readonly DbContextOptions<TContext> _dbContextOptions;
        
        protected DbRepositoryTestBase(string databaseName)
        {
            _mockLogger = new Mock<ILogger<TRepositoryImpl>>();
            _dbContextOptions = CreateDbContextOptions(databaseName);
        }
        
        protected abstract DbContextOptions<TContext> CreateDbContextOptions(string databaseName);
        protected abstract Task<TContext> GetDbContextAsync();
        protected abstract Task<TRepositoryImpl> CreateRepositoryInstanceAsync(TContext context);
        
        protected override async Task<TRepository> CreateRepositoryAsync()
        {
            var context = await GetDbContextAsync();
            return await CreateRepositoryInstanceAsync(context);
        }
    }
} 