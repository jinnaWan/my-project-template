using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Data;
using MyProject.Api.Data.Repositories;
using MyProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Data
{
    /// <summary>
    /// Database-backed tests for Todo repository
    /// </summary>
    public class TodoRepositoryTests 
        : DbRepositoryTestBase<Todo, ITodoRepository, TestTodoRepository, ApplicationDbContext>
    {
        public TodoRepositoryTests() 
            : base($"TodoDb_{Guid.NewGuid()}")
        {
        }

        protected override DbContextOptions<ApplicationDbContext> CreateDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        protected override async Task<ApplicationDbContext> GetDbContextAsync()
        {
            var context = new ApplicationDbContext(_dbContextOptions);
            await context.Database.EnsureCreatedAsync();
            
            if (!context.Todos.Any())
            {
                context.Todos.AddRange(new List<Todo>
                {
                    new Todo 
                    { 
                        Id = 1, 
                        Title = "Test Todo 1", 
                        Description = "Test Description 1", 
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new Todo 
                    { 
                        Id = 2, 
                        Title = "Test Todo 2", 
                        Description = "Test Description 2", 
                        IsCompleted = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        UpdatedAt = DateTime.UtcNow.AddHours(-12)
                    },
                    new Todo 
                    { 
                        Id = 3, 
                        Title = "Test Todo 3", 
                        Description = "Test Description 3", 
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow.AddHours(-6),
                        UpdatedAt = DateTime.UtcNow.AddHours(-6)
                    }
                });
                await context.SaveChangesAsync();
            }
            
            return context;
        }

        protected override async Task<TestTodoRepository> CreateRepositoryInstanceAsync(ApplicationDbContext context)
        {
            return await Task.FromResult(new TestTodoRepository(context, _mockLogger.Object));
        }

        protected override Todo CreateNewEntity()
        {
            return new Todo
            {
                Title = "New Todo",
                Description = "New Description",
                IsCompleted = false
            };
        }

        protected override int GetValidId() => 2;

        protected override int GetInvalidId() => 999;

        protected override void UpdateEntity(Todo entity)
        {
            entity.Title = "Updated Title";
            entity.Description = "Updated Description";
            entity.IsCompleted = true;
        }

        protected override void AssertEntityProperties(Todo expected, Todo actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.IsCompleted, actual.IsCompleted);
        }

        [Fact]
        public async Task GetCompletedAsync_ReturnsOnlyCompletedTodos()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repository = await CreateRepositoryInstanceAsync(context);

            // Act
            var completedTodos = await repository.GetCompletedAsync();

            // Assert
            var todoList = completedTodos.ToList();
            Assert.Single(todoList);
            Assert.All(todoList, todo => Assert.True(todo.IsCompleted));
            Assert.Contains(todoList, t => t.Id == 2);
        }

        [Fact]
        public async Task GetIncompleteAsync_ReturnsOnlyIncompleteTodos()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repository = await CreateRepositoryInstanceAsync(context);

            // Act
            var incompleteTodos = await repository.GetIncompleteAsync();

            // Assert
            var todoList = incompleteTodos.ToList();
            Assert.Equal(2, todoList.Count);
            Assert.All(todoList, todo => Assert.False(todo.IsCompleted));
            Assert.Contains(todoList, t => t.Id == 1);
            Assert.Contains(todoList, t => t.Id == 3);
        }

        [Fact]
        public async Task MarkAsCompletedAsync_WithValidId_UpdatesAndReturnsTrue()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repository = await CreateRepositoryInstanceAsync(context);

            // Act
            var result = await repository.MarkAsCompletedAsync(1);

            // Assert
            Assert.True(result);
            
            // Verify it's updated in the database
            var todo = await context.Todos.FindAsync(1);
            Assert.True(todo.IsCompleted);
        }

        [Fact]
        public async Task MarkAsIncompleteAsync_WithValidId_UpdatesAndReturnsTrue()
        {
            // Arrange
            var context = await GetDbContextAsync();
            var repository = await CreateRepositoryInstanceAsync(context);

            // Act
            var result = await repository.MarkAsIncompleteAsync(2);

            // Assert
            Assert.True(result);
            
            // Verify it's updated in the database
            var todo = await context.Todos.FindAsync(2);
            Assert.False(todo.IsCompleted);
        }
    }
} 