using Moq;
using MyProject.Api.Data;
using MyProject.Api.Models;
using MyProject.Api.Services;
using System;
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
    }
} 