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
    /// Mock-based tests for Todo repository
    /// </summary>
    public class TodoRepositoryMockTests : MockRepositoryTestBase<Todo, ITodoRepository, TodoRepository>
    {
        public TodoRepositoryMockTests() 
            : base(CreateTestTodos())
        {
            // Setup specific mock methods for ITodoRepository
            _mockRepository.Setup(repo => repo.GetByIdAsync(2))
                .ReturnsAsync(_testEntities.FirstOrDefault(t => t.Id == 2));
                
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Todo)null);
                
            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Todo>()))
                .ReturnsAsync((Todo todo) => {
                    var newTodo = new Todo
                    {
                        Id = 4,
                        Title = todo.Title,
                        Description = todo.Description,
                        IsCompleted = todo.IsCompleted,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    return newTodo;
                });
                
            _mockRepository.Setup(repo => repo.UpdateAsync(It.Is<Todo>(t => t.Id != 999)))
                .ReturnsAsync(true);
                
            _mockRepository.Setup(repo => repo.UpdateAsync(It.Is<Todo>(t => t.Id == 999)))
                .ReturnsAsync(false);
                
            _mockRepository.Setup(repo => repo.GetCompletedAsync())
                .ReturnsAsync(_testEntities.Where(t => t.IsCompleted).ToList());
                
            _mockRepository.Setup(repo => repo.GetIncompleteAsync())
                .ReturnsAsync(_testEntities.Where(t => !t.IsCompleted).ToList());
                
            _mockRepository.Setup(repo => repo.MarkAsCompletedAsync(1))
                .ReturnsAsync(true);
                
            _mockRepository.Setup(repo => repo.MarkAsIncompleteAsync(2))
                .ReturnsAsync(true);
        }

        private static List<Todo> CreateTestTodos()
        {
            return new List<Todo>
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
            };
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
            // Act
            var completedTodos = await _mockRepository.Object.GetCompletedAsync();

            // Assert
            var todoList = completedTodos.ToList();
            Assert.Single(todoList);
            Assert.All(todoList, todo => Assert.True(todo.IsCompleted));
            Assert.Contains(todoList, t => t.Id == 2);
        }

        [Fact]
        public async Task GetIncompleteAsync_ReturnsOnlyIncompleteTodos()
        {
            // Act
            var incompleteTodos = await _mockRepository.Object.GetIncompleteAsync();

            // Assert
            var todoList = incompleteTodos.ToList();
            Assert.Equal(2, todoList.Count);
            Assert.All(todoList, todo => Assert.False(todo.IsCompleted));
            Assert.Contains(todoList, t => t.Id == 1);
            Assert.Contains(todoList, t => t.Id == 3);
        }

        [Fact]
        public async Task MarkAsCompletedAsync_WithValidId_ReturnsTrue()
        {
            // Act
            var result = await _mockRepository.Object.MarkAsCompletedAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MarkAsIncompleteAsync_WithValidId_ReturnsTrue()
        {
            // Act
            var result = await _mockRepository.Object.MarkAsIncompleteAsync(2);

            // Assert
            Assert.True(result);
        }
    }
} 