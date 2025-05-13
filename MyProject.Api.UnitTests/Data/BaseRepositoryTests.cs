using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Data
{
    // Concrete implementation of TestRepositoryBase for testing
    public class TestEntityRepository : TestRepositoryBase<TestEntity>
    {
        public TestEntityRepository(TestDbContext dbContext, ILogger<TestEntityRepository> logger) 
            : base(dbContext, logger)
        {
        }
    }

    public class BaseRepositoryTests
    {
        private readonly Mock<ILogger<TestEntityRepository>> _mockLogger;
        private readonly DbContextOptions<TestDbContext> _dbContextOptions;

        public BaseRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<TestEntityRepository>>();
            _dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestEntityDb_{Guid.NewGuid()}")
                .Options;
        }

        private async Task<TestDbContext> GetDbContext()
        {
            var context = new TestDbContext(_dbContextOptions);
            await context.Database.EnsureCreatedAsync();
            
            if (!context.TestEntities.Any())
            {
                context.TestEntities.AddRange(new List<TestEntity>
                {
                    new TestEntity 
                    { 
                        Id = 1, 
                        Name = "Test Entity 1",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new TestEntity 
                    { 
                        Id = 2, 
                        Name = "Test Entity 2",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        UpdatedAt = DateTime.UtcNow.AddHours(-12)
                    },
                    new TestEntity 
                    { 
                        Id = 3, 
                        Name = "Test Entity 3",
                        CreatedAt = DateTime.UtcNow.AddHours(-6),
                        UpdatedAt = DateTime.UtcNow.AddHours(-6)
                    }
                });
                await context.SaveChangesAsync();
            }
            
            return context;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestEntityRepository(context, _mockLogger.Object);

            // Act
            var entities = await repository.GetAllAsync();

            // Assert
            var entityList = entities.ToList();
            Assert.Equal(3, entityList.Count);
            Assert.Contains(entityList, e => e.Id == 1);
            Assert.Contains(entityList, e => e.Id == 2);
            Assert.Contains(entityList, e => e.Id == 3);
        }

        [Fact]
        public async Task FindAsync_WithFilter_ReturnsFilteredEntities()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestEntityRepository(context, _mockLogger.Object);
            Expression<Func<TestEntity, bool>> filter = e => e.Name.Contains("Entity 2");

            // Act
            var entities = await repository.FindAsync(filter);

            // Assert
            var entityList = entities.ToList();
            Assert.Single(entityList);
            Assert.Equal(2, entityList[0].Id);
            Assert.Equal("Test Entity 2", entityList[0].Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsEntity()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestEntityRepository(context, _mockLogger.Object);

            // Act
            var entity = await repository.GetByIdAsync(2);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(2, entity.Id);
            Assert.Equal("Test Entity 2", entity.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestEntityRepository(context, _mockLogger.Object);

            // Act
            var entity = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(entity);
        }

        [Fact]
        public async Task AddAsync_AddsEntityAndReturnsWithId()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestEntityRepository(context, _mockLogger.Object);
            var newEntity = new TestEntity
            {
                Name = "New Test Entity",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = await repository.AddAsync(newEntity);

            // Assert
            Assert.NotEqual(0, result.Id);
            Assert.Equal("New Test Entity", result.Name);

            // Verify it's in the database
            var dbEntity = await context.TestEntities.FindAsync(result.Id);
            Assert.NotNull(dbEntity);
            Assert.Equal(result.Id, dbEntity.Id);
        }

        [Fact]
        public async Task UpdateAsync_WithValidEntity_UpdatesAndReturnsTrue()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestEntityRepository(context, _mockLogger.Object);
            var entityToUpdate = await context.TestEntities.FindAsync(1);
            entityToUpdate.Name = "Updated Entity";
            entityToUpdate.UpdatedAt = DateTime.UtcNow;

            // Act
            var result = await repository.UpdateAsync(entityToUpdate);

            // Assert
            Assert.True(result);
            
            // Verify it's updated in the database
            var dbEntity = await context.TestEntities.FindAsync(1);
            Assert.Equal("Updated Entity", dbEntity.Name);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesAndReturnsTrue()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestEntityRepository(context, _mockLogger.Object);

            // Act
            var result = await repository.DeleteAsync(3);

            // Assert
            Assert.True(result);
            
            // Verify it's deleted from the database
            var dbEntity = await context.TestEntities.FindAsync(3);
            Assert.Null(dbEntity);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestEntityRepository(context, _mockLogger.Object);

            // Act
            var result = await repository.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
} 