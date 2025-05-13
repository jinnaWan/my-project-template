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
    public class UserRepositoryTests
    {
        private readonly Mock<ILogger<TestUserRepository>> _mockLogger;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public UserRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<TestUserRepository>>();
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"UserDb_{Guid.NewGuid()}")
                .Options;
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var context = new ApplicationDbContext(_dbContextOptions);
            await context.Database.EnsureCreatedAsync();
            
            if (!context.Users.Any())
            {
                context.Users.AddRange(new List<User>
                {
                    new User 
                    { 
                        Id = 1, 
                        Username = "johndoe",
                        Email = "john.doe@example.com",
                        FirstName = "John",
                        LastName = "Doe",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new User 
                    { 
                        Id = 2, 
                        Username = "janesmith",
                        Email = "jane.smith@example.com",
                        FirstName = "Jane",
                        LastName = "Smith",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-8),
                        UpdatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new User 
                    { 
                        Id = 3, 
                        Username = "bobwilliams",
                        Email = "bob.williams@example.com",
                        FirstName = "Bob",
                        LastName = "Williams",
                        IsActive = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-15),
                        UpdatedAt = DateTime.UtcNow.AddDays(-2)
                    }
                });
                await context.SaveChangesAsync();
            }
            
            return context;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var users = await repository.GetAllAsync();

            // Assert
            var userList = users.ToList();
            Assert.Equal(3, userList.Count);
            Assert.Contains(userList, u => u.Id == 1);
            Assert.Contains(userList, u => u.Id == 2);
            Assert.Contains(userList, u => u.Id == 3);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsUser()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var user = await repository.GetByIdAsync(2);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(2, user.Id);
            Assert.Equal("janesmith", user.Username);
            Assert.Equal("jane.smith@example.com", user.Email);
            Assert.Equal("Jane", user.FirstName);
            Assert.Equal("Smith", user.LastName);
            Assert.True(user.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var user = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithValidUsername_ReturnsUser()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var user = await repository.GetByUsernameAsync("johndoe");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("johndoe", user.Username);
            Assert.Equal("john.doe@example.com", user.Email);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithInvalidUsername_ReturnsNull()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var user = await repository.GetByUsernameAsync("nonexistent");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var user = await repository.GetByEmailAsync("bob.williams@example.com");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(3, user.Id);
            Assert.Equal("bobwilliams", user.Username);
            Assert.Equal("bob.williams@example.com", user.Email);
            Assert.False(user.IsActive);
        }

        [Fact]
        public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var user = await repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task GetActiveUsersAsync_ReturnsOnlyActiveUsers()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var users = await repository.GetActiveUsersAsync();

            // Assert
            var userList = users.ToList();
            Assert.Equal(2, userList.Count);
            Assert.All(userList, user => Assert.True(user.IsActive));
            Assert.Contains(userList, u => u.Id == 1);
            Assert.Contains(userList, u => u.Id == 2);
            Assert.DoesNotContain(userList, u => u.Id == 3);
        }

        [Fact]
        public async Task AddAsync_AddsUserAndReturnsWithId()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);
            var newUser = new User
            {
                Username = "newuser",
                Email = "new.user@example.com",
                FirstName = "New",
                LastName = "User",
                IsActive = true
            };

            // Act
            var result = await repository.AddAsync(newUser);

            // Assert
            Assert.NotEqual(0, result.Id);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("new.user@example.com", result.Email);
            Assert.Equal("New", result.FirstName);
            Assert.Equal("User", result.LastName);
            Assert.True(result.IsActive);
            Assert.NotEqual(default, result.CreatedAt);
            Assert.NotEqual(default, result.UpdatedAt);

            // Verify it's in the database
            var dbUser = await context.Users.FindAsync(result.Id);
            Assert.NotNull(dbUser);
            Assert.Equal(result.Id, dbUser.Id);
        }

        [Fact]
        public async Task UpdateAsync_WithValidUser_UpdatesAndReturnsTrue()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);
            var userToUpdate = await context.Users.FindAsync(1);
            userToUpdate.Username = "johndoe_updated";
            userToUpdate.Email = "john.updated@example.com";
            userToUpdate.FirstName = "John Updated";
            userToUpdate.LastName = "Doe Updated";

            // Act
            var result = await repository.UpdateAsync(userToUpdate);

            // Assert
            Assert.True(result);
            
            // Verify it's updated in the database
            var dbUser = await context.Users.FindAsync(1);
            Assert.Equal("johndoe_updated", dbUser.Username);
            Assert.Equal("john.updated@example.com", dbUser.Email);
            Assert.Equal("John Updated", dbUser.FirstName);
            Assert.Equal("Doe Updated", dbUser.LastName);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);
            var userToUpdate = new User
            {
                Id = 999,
                Username = "invalid",
                Email = "invalid@example.com",
                FirstName = "Invalid",
                LastName = "User"
            };

            // Act
            var result = await repository.UpdateAsync(userToUpdate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesAndReturnsTrue()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var result = await repository.DeleteAsync(3);

            // Assert
            Assert.True(result);
            
            // Verify it's deleted from the database
            var dbUser = await context.Users.FindAsync(3);
            Assert.Null(dbUser);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var result = await repository.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ActivateUserAsync_WithValidId_UpdatesAndReturnsTrue()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var result = await repository.ActivateUserAsync(3); // Bob is inactive

            // Assert
            Assert.True(result);
            
            // Verify it's updated in the database
            var dbUser = await context.Users.FindAsync(3);
            Assert.True(dbUser.IsActive);
        }

        [Fact]
        public async Task DeactivateUserAsync_WithValidId_UpdatesAndReturnsTrue()
        {
            // Arrange
            using var context = await GetDbContext();
            var repository = new TestUserRepository(context, _mockLogger.Object);

            // Act
            var result = await repository.DeactivateUserAsync(1); // John is active

            // Assert
            Assert.True(result);
            
            // Verify it's updated in the database
            var dbUser = await context.Users.FindAsync(1);
            Assert.False(dbUser.IsActive);
        }
    }
} 