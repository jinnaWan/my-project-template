using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Data.Repositories;
using MyProject.Api.Models;
using MyProject.Api.Services;
using MyProject.Api.UnitTests.TestUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Services
{
    public class TodoServiceTests : IDisposable
    {
        private readonly ServiceTestFixture<TodoService> _fixture;

        public TodoServiceTests()
        {
            _fixture = new ServiceTestFixture<TodoService>();
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }

        // Helper properties for better readability
        private Mock<ITodoRepository> Repository => _fixture.GetMock<ITodoRepository>();
        private Mock<ILogger<TodoService>> Logger => _fixture.GetMock<ILogger<TodoService>>();
        private TodoService Service => _fixture.Service;

        // Async Tests

        [Fact]
        public async Task GetAllTodosAsync_ShouldReturnAllTodos()
        {
            // Arrange
            var expectedTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Test Todo 1", IsCompleted = false },
                new Todo { Id = 2, Title = "Test Todo 2", IsCompleted = true }
            };
            Repository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await Service.GetAllTodosAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Repository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithValidId_ShouldReturnTodo()
        {
            // Arrange
            var expectedTodo = new Todo { Id = 1, Title = "Test Todo", IsCompleted = false };
            Repository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await Service.GetTodoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Todo", result.Title);
            Repository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateTodoAsync_WithValidTodo_ShouldReturnCreatedTodo()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "New Todo", IsCompleted = false };
            var expectedTodo = new Todo { Id = 1, Title = "New Todo", IsCompleted = false };
            
            Repository.Setup(repo => repo.AddAsync(It.IsAny<Todo>()))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await Service.CreateTodoAsync(todoToCreate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Todo", result.Title);
            Repository.Verify(repo => repo.AddAsync(It.IsAny<Todo>()), Times.Once);
        }

        [Fact]
        public async Task CreateTodoAsync_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "", IsCompleted = false };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => Service.CreateTodoAsync(todoToCreate));
            Assert.Contains("title cannot be empty", exception.Message.ToLower());
        }

        [Fact]
        public async Task UpdateTodoAsync_WithValidTodo_ShouldReturnTrue()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "Updated Todo", IsCompleted = true };
            
            Repository.Setup(repo => repo.UpdateAsync(It.IsAny<Todo>()))
                .ReturnsAsync(true);

            // Act
            var result = await Service.UpdateTodoAsync(todoToUpdate);

            // Assert
            Assert.True(result);
            Repository.Verify(repo => repo.UpdateAsync(It.IsAny<Todo>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "", IsCompleted = true };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => Service.UpdateTodoAsync(todoToUpdate));
            Assert.Contains("title cannot be empty", exception.Message.ToLower());
        }

        [Fact]
        public async Task DeleteTodoAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            Repository.Setup(repo => repo.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await Service.DeleteTodoAsync(1);

            // Assert
            Assert.True(result);
            Repository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            Repository.Setup(repo => repo.DeleteAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await Service.DeleteTodoAsync(999);

            // Assert
            Assert.False(result);
            Repository.Verify(repo => repo.DeleteAsync(999), Times.Once);
        }
        
        [Fact]
        public async Task GetCompletedTodosAsync_ShouldReturnCompletedTodos()
        {
            // Arrange
            var expectedTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Test Todo 1", IsCompleted = true },
                new Todo { Id = 2, Title = "Test Todo 2", IsCompleted = true }
            };
            Repository.Setup(repo => repo.GetCompletedAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await Service.GetCompletedTodosAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, todo => Assert.True(todo.IsCompleted));
            Repository.Verify(repo => repo.GetCompletedAsync(), Times.Once);
        }
        
        [Fact]
        public async Task GetIncompleteTodosAsync_ShouldReturnIncompleteTodos()
        {
            // Arrange
            var expectedTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Test Todo 1", IsCompleted = false },
                new Todo { Id = 2, Title = "Test Todo 2", IsCompleted = false }
            };
            Repository.Setup(repo => repo.GetIncompleteAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await Service.GetIncompleteTodosAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, todo => Assert.False(todo.IsCompleted));
            Repository.Verify(repo => repo.GetIncompleteAsync(), Times.Once);
        }
        
        [Fact]
        public async Task MarkAsCompletedAsync_ShouldReturnTrue()
        {
            // Arrange
            Repository.Setup(repo => repo.MarkAsCompletedAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await Service.MarkAsCompletedAsync(1);

            // Assert
            Assert.True(result);
            Repository.Verify(repo => repo.MarkAsCompletedAsync(1), Times.Once);
        }
        
        [Fact]
        public async Task MarkAsIncompleteAsync_ShouldReturnTrue()
        {
            // Arrange
            Repository.Setup(repo => repo.MarkAsIncompleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await Service.MarkAsIncompleteAsync(1);

            // Assert
            Assert.True(result);
            Repository.Verify(repo => repo.MarkAsIncompleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllTodosAsync_HandlesException_LogsError()
        {
            // Arrange
            Repository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => Service.GetAllTodosAsync());
            Logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error getting all todos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
        
        [Fact]
        public async Task GetTodoByIdAsync_HandlesException_LogsError()
        {
            // Arrange
            Repository.Setup(repo => repo.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => Service.GetTodoByIdAsync(1));
            Logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error getting todo with ID 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
        
        [Fact]
        public async Task CreateTodoAsync_HandlesException_LogsError()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "New Todo", IsCompleted = false };
            
            Repository.Setup(repo => repo.AddAsync(It.IsAny<Todo>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => Service.CreateTodoAsync(todoToCreate));
            Logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error creating todo with title")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
} 