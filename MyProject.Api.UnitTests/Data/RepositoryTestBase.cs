using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Data.Repositories;
using MyProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Data
{
    /// <summary>
    /// Base class for repository tests with common test functionality
    /// for both mock-based and database-based tests
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TRepository">The repository interface type</typeparam>
    public abstract class RepositoryTestBase<TEntity, TRepository> 
        where TEntity : class 
        where TRepository : class, IRepository<TEntity>
    {
        protected abstract Task<TRepository> CreateRepositoryAsync();
        protected abstract TEntity CreateNewEntity();
        protected abstract int GetValidId();
        protected abstract int GetInvalidId();
        protected abstract void UpdateEntity(TEntity entity);
        protected abstract void AssertEntityProperties(TEntity expected, TEntity actual);
        
        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            var repository = await CreateRepositoryAsync();

            // Act
            var entities = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(entities);
            Assert.NotEmpty(entities);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsEntity()
        {
            // Arrange
            var repository = await CreateRepositoryAsync();
            var validId = GetValidId();

            // Act
            var entity = await repository.GetByIdAsync(validId);

            // Assert
            Assert.NotNull(entity);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var repository = await CreateRepositoryAsync();
            var invalidId = GetInvalidId();

            // Act
            var entity = await repository.GetByIdAsync(invalidId);

            // Assert
            Assert.Null(entity);
        }

        [Fact]
        public async Task AddAsync_AddsEntityAndReturnsWithId()
        {
            // Arrange
            var repository = await CreateRepositoryAsync();
            var newEntity = CreateNewEntity();

            // Act
            var result = await repository.AddAsync(newEntity);

            // Assert
            Assert.NotEqual(0, typeof(TEntity).GetProperty("Id")?.GetValue(result));
        }

        [Fact]
        public async Task UpdateAsync_WithValidEntity_UpdatesAndReturnsTrue()
        {
            // Arrange
            var repository = await CreateRepositoryAsync();
            var validId = GetValidId();
            var entity = await repository.GetByIdAsync(validId);
            
            if (entity != null)
            {
                UpdateEntity(entity);

                // Act
                var result = await repository.UpdateAsync(entity);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesAndReturnsTrue()
        {
            // Arrange
            var repository = await CreateRepositoryAsync();
            var validId = GetValidId();

            // Act
            var result = await repository.DeleteAsync(validId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var repository = await CreateRepositoryAsync();
            var invalidId = GetInvalidId();

            // Act
            var result = await repository.DeleteAsync(invalidId);

            // Assert
            Assert.False(result);
        }
    }
} 