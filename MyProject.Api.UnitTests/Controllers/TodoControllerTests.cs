using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Controllers;
using MyProject.Api.Models;
using MyProject.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Controllers
{
    public class TodoControllerTests : IDisposable
    {
        private readonly Mock<ITodoService> _mockTodoService;
        private readonly Mock<ILogger<TodoController>> _mockLogger;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            _mockTodoService = new Mock<ITodoService>();
            _mockLogger = new Mock<ILogger<TodoController>>();
            _controller = new TodoController(_mockTodoService.Object, _mockLogger.Object);
        }

        public void Dispose()
        {
            // Clean up if needed
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult_WithTodoList()
        {
            // Arrange
            var expectedTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Test Todo 1", IsCompleted = false },
                new Todo { Id = 2, Title = "Test Todo 2", IsCompleted = true }
            };
            _mockTodoService.Setup(service => service.GetAllTodosAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodos = Assert.IsAssignableFrom<IEnumerable<Todo>>(okResult.Value);
            Assert.Equal(2, returnedTodos.Count());
        }

        [Fact]
        public async Task GetAllAsync_HandlesException_ReturnsServerError()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetAllTodosAsync())
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An unexpected error occurred", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var expectedTodo = new Todo { Id = 1, Title = "Test Todo", IsCompleted = false };
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(1))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodo = Assert.IsType<Todo>(okResult.Value);
            Assert.Equal(1, returnedTodo.Id);
            Assert.Equal("Test Todo", returnedTodo.Title);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(999))
                .ReturnsAsync((Todo)null);

            // Act
            var result = await _controller.GetByIdAsync(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetByIdAsync_HandlesException_ReturnsServerError()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetTodoByIdAsync(1))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetByIdAsync(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An unexpected error occurred", statusCodeResult.Value);
        }

        [Fact]
        public async Task CreateAsync_WithValidTodo_ReturnsCreatedAtRouteResult()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "New Todo", IsCompleted = false };
            var createdTodo = new Todo { Id = 1, Title = "New Todo", IsCompleted = false };
            
            _mockTodoService.Setup(service => service.CreateTodoAsync(It.IsAny<Todo>()))
                .ReturnsAsync(createdTodo);

            // Act
            var result = await _controller.CreateAsync(todoToCreate);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal("GetTodoById", createdAtRouteResult.RouteName);
            Assert.Equal(1, createdAtRouteResult.RouteValues["id"]);
            var returnedTodo = Assert.IsType<Todo>(createdAtRouteResult.Value);
            Assert.Equal(1, returnedTodo.Id);
            Assert.Equal("New Todo", returnedTodo.Title);
        }

        [Fact]
        public async Task CreateAsync_WithArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "", IsCompleted = false };
            
            _mockTodoService.Setup(service => service.CreateTodoAsync(It.IsAny<Todo>()))
                .ThrowsAsync(new ArgumentException("Title cannot be empty"));

            // Act
            var result = await _controller.CreateAsync(todoToCreate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Title cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAsync_HandlesException_ReturnsServerError()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "Test Todo", IsCompleted = false };
            
            _mockTodoService.Setup(service => service.CreateTodoAsync(It.IsAny<Todo>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateAsync(todoToCreate);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An unexpected error occurred", statusCodeResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_WithValidIdAndTodo_ReturnsNoContent()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "Updated Todo", IsCompleted = true };
            
            _mockTodoService.Setup(service => service.UpdateTodoAsync(It.IsAny<Todo>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(1, todoToUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_WithMismatchedIds_ReturnsBadRequest()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 2, Title = "Updated Todo", IsCompleted = true };
            
            // Act
            var result = await _controller.UpdateAsync(1, todoToUpdate);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_WithNotFoundTodo_ReturnsNotFound()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 999, Title = "Updated Todo", IsCompleted = true };
            
            _mockTodoService.Setup(service => service.UpdateTodoAsync(It.IsAny<Todo>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateAsync(999, todoToUpdate);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_WithArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "", IsCompleted = true };
            
            _mockTodoService.Setup(service => service.UpdateTodoAsync(It.IsAny<Todo>()))
                .ThrowsAsync(new ArgumentException("Title cannot be empty"));

            // Act
            var result = await _controller.UpdateAsync(1, todoToUpdate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Title cannot be empty", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAsync_HandlesException_ReturnsServerError()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "Updated Todo", IsCompleted = true };
            
            _mockTodoService.Setup(service => service.UpdateTodoAsync(It.IsAny<Todo>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateAsync(1, todoToUpdate);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An unexpected error occurred", statusCodeResult.Value);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _mockTodoService.Setup(service => service.DeleteTodoAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAsync(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTodoService.Setup(service => service.DeleteTodoAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAsync(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_HandlesException_ReturnsServerError()
        {
            // Arrange
            _mockTodoService.Setup(service => service.DeleteTodoAsync(1))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.DeleteAsync(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An unexpected error occurred", statusCodeResult.Value);
        }
    }
} 