using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Api.UnitTests.Data
{
    /// <summary>
    /// Base class for repository tests using mocks
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TRepository">The repository interface type</typeparam>
    /// <typeparam name="TRepositoryImpl">The repository implementation type</typeparam>
    public abstract class MockRepositoryTestBase<TEntity, TRepository, TRepositoryImpl> : RepositoryTestBase<TEntity, TRepository>
        where TEntity : class
        where TRepository : class, IRepository<TEntity>
        where TRepositoryImpl : class
    {
        protected readonly Mock<TRepository> _mockRepository;
        protected readonly Mock<ILogger<TRepositoryImpl>> _mockLogger;
        protected readonly List<TEntity> _testEntities;

        protected MockRepositoryTestBase(List<TEntity> testEntities)
        {
            _mockRepository = new Mock<TRepository>();
            _mockLogger = new Mock<ILogger<TRepositoryImpl>>();
            _testEntities = testEntities;
            
            // Setup standard mock repository methods
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(_testEntities);
                
            _mockRepository.Setup(repo => repo.DeleteAsync(GetInvalidId()))
                .ReturnsAsync(false);
                
            _mockRepository.Setup(repo => repo.DeleteAsync(GetValidId()))
                .ReturnsAsync(true);
        }

        protected override async Task<TRepository> CreateRepositoryAsync()
        {
            return await Task.FromResult(_mockRepository.Object);
        }
    }
} 