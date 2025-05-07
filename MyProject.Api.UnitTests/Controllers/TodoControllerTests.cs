using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Api.Controllers;
using MyProject.Api.Models;
using MyProject.Api.Services;
using System.Collections.Generic;
using Xunit;

namespace MyProject.Api.UnitTests.Controllers
{
    public class TodoControllerTests
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

        [Fact]
        public void GetAll_ReturnsOkResult_WithTodoList()
        {
            // Arrange
            var expectedTodos = new List<Todo>
            {
                new Todo { Id = 1, Title = "Test Todo 1", IsCompleted = false },
                new Todo { Id = 2, Title = "Test Todo 2", IsCompleted = true }
            };
            _mockTodoService.Setup(service => service.GetAllTodos())
                .Returns(expectedTodos);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodos = Assert.IsAssignableFrom<IEnumerable<Todo>>(okResult.Value);
            Assert.Equal(2, returnedTodos.Count());
        }

        [Fact]
        public void GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var expectedTodo = new Todo { Id = 1, Title = "Test Todo", IsCompleted = false };
            _mockTodoService.Setup(service => service.GetTodoById(1))
                .Returns(expectedTodo);

            // Act
            var result = _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTodo = Assert.IsType<Todo>(okResult.Value);
            Assert.Equal(1, returnedTodo.Id);
            Assert.Equal("Test Todo", returnedTodo.Title);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTodoService.Setup(service => service.GetTodoById(999))
                .Returns((Todo)null);

            // Act
            var result = _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void Create_WithValidTodo_ReturnsCreatedResult()
        {
            // Arrange
            var todoToCreate = new Todo { Title = "New Todo", IsCompleted = false };
            var createdTodo = new Todo { Id = 1, Title = "New Todo", IsCompleted = false };
            
            _mockTodoService.Setup(service => service.CreateTodo(It.IsAny<Todo>()))
                .Returns(createdTodo);

            // Act
            var result = _controller.Create(todoToCreate);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TodoController.GetById), createdAtActionResult.ActionName);
            var returnedTodo = Assert.IsType<Todo>(createdAtActionResult.Value);
            Assert.Equal(1, returnedTodo.Id);
            Assert.Equal("New Todo", returnedTodo.Title);
        }

        [Fact]
        public void Update_WithValidIdAndTodo_ReturnsNoContent()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 1, Title = "Updated Todo", IsCompleted = true };
            
            _mockTodoService.Setup(service => service.UpdateTodo(It.IsAny<Todo>()))
                .Returns(true);

            // Act
            var result = _controller.Update(1, todoToUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Update_WithMismatchedIds_ReturnsBadRequest()
        {
            // Arrange
            var todoToUpdate = new Todo { Id = 2, Title = "Updated Todo", IsCompleted = true };
            
            // Act
            var result = _controller.Update(1, todoToUpdate);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _mockTodoService.Setup(service => service.DeleteTodo(1))
                .Returns(true);

            // Act
            var result = _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockTodoService.Setup(service => service.DeleteTodo(999))
                .Returns(false);

            // Act
            var result = _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
} 