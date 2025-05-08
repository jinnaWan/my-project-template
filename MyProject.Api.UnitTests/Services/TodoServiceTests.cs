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
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _mockRepository = new Mock<ITodoRepository>();
            _service = new TodoService(_mockRepository.Object);
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

        // Synchronous Tests

        [Fact]
        public void GetAllTodos_ShouldReturnAllTodos()
        {
            // Arrange
            var expectedTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Test Todo 1", IsCompleted = false },
                new Todo { Id = 2, Title = "Test Todo 2", IsCompleted = true }
            };
            _mockRepository.Setup(repo => repo.GetAll())
                .Returns(expectedTodos);

            // Act
            var result = _service.GetAllTodos();

            // Assert
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void GetTodoById_WithValidId_ShouldReturnTodo()
        {
            // Arrange
            var expectedTodo = new Todo { Id = 1, Title = "Test Todo", IsCompleted = false };
            _mockRepository.Setup(repo => repo.GetById(1))
                .Returns(expectedTodo);

            // Act
            var result = _service.GetTodoById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Todo", result.Title);
            _mockRepository.Verify(repo => repo.GetById(1), Times.Once);
        }

        [Fact]
        public void CreateTodo_WithValidTodo_ShouldReturnCreatedTodo()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "New Todo", IsCompleted = false };
            var expectedTodo = new Todo { Id = 1, Title = "New Todo", IsCompleted = false };
            
            _mockRepository.Setup(repo => repo.Add(It.IsAny<Todo>()))
                .Returns(expectedTodo);

            // Act
            var result = _service.CreateTodo(todoToCreate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Todo", result.Title);
            _mockRepository.Verify(repo => repo.Add(It.IsAny<Todo>()), Times.Once);
        }

        [Fact]
        public void CreateTodo_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "", IsCompleted = false };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.CreateTodo(todoToCreate));
            Assert.Contains("title cannot be empty", exception.Message.ToLower());
        }

        [Fact]
        public void UpdateTodo_WithValidTodo_ShouldReturnTrue()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "Updated Todo", IsCompleted = true };
            
            _mockRepository.Setup(repo => repo.Update(It.IsAny<Todo>()))
                .Returns(true);

            // Act
            var result = _service.UpdateTodo(todoToUpdate);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.Update(It.IsAny<Todo>()), Times.Once);
        }

        [Fact]
        public void UpdateTodo_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "", IsCompleted = true };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.UpdateTodo(todoToUpdate));
            Assert.Contains("title cannot be empty", exception.Message.ToLower());
        }

        [Fact]
        public void DeleteTodo_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.Delete(1))
                .Returns(true);

            // Act
            var result = _service.DeleteTodo(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.Delete(1), Times.Once);
        }

        [Fact]
        public void DeleteTodo_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.Delete(999))
                .Returns(false);

            // Act
            var result = _service.DeleteTodo(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.Delete(999), Times.Once);
        }
        
        [Fact]
        public void GetCompletedTodos_ShouldReturnCompletedTodos()
        {
            // Arrange
            var expectedTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Test Todo 1", IsCompleted = true },
                new Todo { Id = 2, Title = "Test Todo 2", IsCompleted = true }
            };
            _mockRepository.Setup(repo => repo.GetCompleted())
                .Returns(expectedTodos);

            // Act
            var result = _service.GetCompletedTodos();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, todo => Assert.True(todo.IsCompleted));
            _mockRepository.Verify(repo => repo.GetCompleted(), Times.Once);
        }
        
        [Fact]
        public void GetIncompleteTodos_ShouldReturnIncompleteTodos()
        {
            // Arrange
            var expectedTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Test Todo 1", IsCompleted = false },
                new Todo { Id = 2, Title = "Test Todo 2", IsCompleted = false }
            };
            _mockRepository.Setup(repo => repo.GetIncomplete())
                .Returns(expectedTodos);

            // Act
            var result = _service.GetIncompleteTodos();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, todo => Assert.False(todo.IsCompleted));
            _mockRepository.Verify(repo => repo.GetIncomplete(), Times.Once);
        }
        
        [Fact]
        public void MarkAsCompleted_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.MarkAsCompleted(1))
                .Returns(true);

            // Act
            var result = _service.MarkAsCompleted(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.MarkAsCompleted(1), Times.Once);
        }
        
        [Fact]
        public void MarkAsIncomplete_ShouldReturnTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.MarkAsIncomplete(1))
                .Returns(true);

            // Act
            var result = _service.MarkAsIncomplete(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.MarkAsIncomplete(1), Times.Once);
        }
    }
} 