using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyProject.Api.Data;
using MyProject.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Data
{
    public class ApplicationDbContextTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public ApplicationDbContextTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public void DbContext_InitializesCorrectly()
        {
            // Arrange & Act
            using var context = new ApplicationDbContext(_dbContextOptions);

            // Assert
            Assert.NotNull(context.Todos);
            Assert.NotNull(context.Users);
        }

        [Fact]
        public async Task EnsureDatabaseCreated_CreatesDatabase()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            
            // Act
            context.EnsureDatabaseCreated();
            
            // Assert
            Assert.True(await context.Database.CanConnectAsync());
        }

        [Fact]
        public async Task RecreateDatabase_DeletesAndCreatesDatabase()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            
            // Add some data
            context.Todos.Add(new Todo { 
                Title = "Test Todo", 
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            
            // Act
            context.RecreateDatabase();
            
            // Assert
            Assert.True(await context.Database.CanConnectAsync());
            Assert.Empty(context.Todos);
        }

        [Fact]
        public void SeedData_AddsSeedDataToEmptyDatabase()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            context.Database.EnsureCreated();
            
            // Act
            context.SeedData();
            
            // Assert
            Assert.Equal(3, context.Todos.Count());
            Assert.Equal(2, context.Users.Count());
            
            // Verify Todo seed data
            Assert.Contains(context.Todos, t => t.Title == "Learn ASP.NET Core");
            Assert.Contains(context.Todos, t => t.Title == "Build Docker containers");
            Assert.Contains(context.Todos, t => t.Title == "Implement CI/CD");
            
            // Verify User seed data
            Assert.Contains(context.Users, u => u.Username == "admin");
            Assert.Contains(context.Users, u => u.Username == "user1");
        }

        [Fact]
        public void SeedData_DoesNotDuplicateExistingData()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            context.Database.EnsureCreated();
            
            // Seed once
            context.SeedData();
            var initialTodoCount = context.Todos.Count();
            var initialUserCount = context.Users.Count();
            
            // Act - Seed again
            context.SeedData();
            
            // Assert - Counts should remain the same
            Assert.Equal(initialTodoCount, context.Todos.Count());
            Assert.Equal(initialUserCount, context.Users.Count());
        }

        [Fact]
        public void OnModelCreating_ConfiguresModelsCorrectly()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            
            // Act & Assert
            var todoEntity = context.Model.FindEntityType(typeof(Todo));
            Assert.NotNull(todoEntity);
            Assert.Equal("Todo", todoEntity.GetTableName());
            
            // Check if Title is required by checking if it's not nullable
            var titleProperty = todoEntity.FindProperty(nameof(Todo.Title));
            Assert.NotNull(titleProperty);
            Assert.False(titleProperty.IsNullable);
            Assert.Equal(200, titleProperty.GetMaxLength());
            
            var userEntity = context.Model.FindEntityType(typeof(User));
            Assert.NotNull(userEntity);
            Assert.Equal("AppUser", userEntity.GetTableName());
            
            // Check if Username is required by checking if it's not nullable
            var usernameProperty = userEntity.FindProperty(nameof(User.Username));
            Assert.NotNull(usernameProperty);
            Assert.False(usernameProperty.IsNullable);
            Assert.Equal(50, usernameProperty.GetMaxLength());
            
            // Check if Email is required by checking if it's not nullable
            var emailProperty = userEntity.FindProperty(nameof(User.Email));
            Assert.NotNull(emailProperty);
            Assert.False(emailProperty.IsNullable);
            Assert.Equal(100, emailProperty.GetMaxLength());
            
            // Verify indexes for uniqueness
            var usernameIndex = userEntity.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(User.Username)));
            Assert.NotNull(usernameIndex);
            Assert.True(usernameIndex.IsUnique);
            
            var emailIndex = userEntity.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(User.Email)));
            Assert.NotNull(emailIndex);
            Assert.True(emailIndex.IsUnique);
        }
    }
} 