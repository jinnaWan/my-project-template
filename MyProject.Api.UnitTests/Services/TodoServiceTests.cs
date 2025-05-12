using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Data.Repositories;
using MyProject.Api.Models;
using MyProject.Api.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Services
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _mockRepository;
        private readonly Mock<ILogger<TodoService>> _mockLogger;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _mockLogger = new Mock<ILogger<TodoService>>();
            _service = new TodoService(_mockRepository.Object, _mockLogger.Object);
        }

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
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _service.GetAllTodosAsync();

            // Assert
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithValidId_ShouldReturnTodo()
        {
            // Arrange
            var expectedTodo = new Todo { Id = 1, Title = "Test Todo", IsCompleted = false };
            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await _service.GetTodoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Todo", result.Title);
            _mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateTodoAsync_WithValidTodo_ShouldReturnCreatedTodo()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "New Todo", IsCompleted = false };
            var expectedTodo = new Todo { Id = 1, Title = "New Todo", IsCompleted = false };
            
            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Todo>()))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await _service.CreateTodoAsync(todoToCreate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Todo", result.Title);
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Todo>()), Times.Once);
        }

        [Fact]
        public async Task CreateTodoAsync_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "", IsCompleted = false };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateTodoAsync(todoToCreate));
            Assert.Contains("title cannot be empty", exception.Message.ToLower());
        }

        [Fact]
        public async Task UpdateTodoAsync_WithValidTodo_ShouldReturnTrue()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "Updated Todo", IsCompleted = true };
            
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Todo>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateTodoAsync(todoToUpdate);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Todo>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "", IsCompleted = true };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateTodoAsync(todoToUpdate));
            Assert.Contains("title cannot be empty", exception.Message.ToLower());
        }

        [Fact]
        public async Task DeleteTodoAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteTodoAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteTodoAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.DeleteAsync(999), Times.Once);
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
            _mockRepository.Setup(repo => repo.GetCompletedAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _service.GetCompletedTodosAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, todo => Assert.True(todo.IsCompleted));
            _mockRepository.Verify(repo => repo.GetCompletedAsync(), Times.Once);
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
            _mockRepository.Setup(repo => repo.GetIncompleteAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _service.GetIncompleteTodosAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, todo => Assert.False(todo.IsCompleted));
            _mockRepository.Verify(repo => repo.GetIncompleteAsync(), Times.Once);
        }
        
        [Fact]
        public async Task MarkAsCompletedAsync_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.MarkAsCompletedAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _service.MarkAsCompletedAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.MarkAsCompletedAsync(1), Times.Once);
        }
        
        [Fact]
        public async Task MarkAsIncompleteAsync_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.MarkAsIncompleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _service.MarkAsIncompleteAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.MarkAsIncompleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllTodosAsync_HandlesException_LogsError()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetAllTodosAsync());
            _mockLogger.Verify(
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
            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetTodoByIdAsync(1));
            _mockLogger.Verify(
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
            
            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Todo>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateTodoAsync(todoToCreate));
            _mockLogger.Verify(
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