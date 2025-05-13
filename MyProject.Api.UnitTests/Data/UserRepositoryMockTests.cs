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
    public class UserRepositoryMockTests
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly Mock<ILogger<UserRepository>> _mockLogger;
        private readonly List<User> _testUsers;

        public UserRepositoryMockTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserRepository>>();
            
            // Setup test data
            _testUsers = new List<User>
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
            };
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(_testUsers);

            // Act
            var result = await _mockRepository.Object.GetAllAsync();

            // Assert
            var users = result.ToList();
            Assert.Equal(3, users.Count);
            Assert.Contains(users, u => u.Id == 1);
            Assert.Contains(users, u => u.Id == 2);
            Assert.Contains(users, u => u.Id == 3);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsUser()
        {
            // Arrange
            var expectedUser = _testUsers.First(u => u.Id == 2);
            _mockRepository.Setup(repo => repo.GetByIdAsync(2))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _mockRepository.Object.GetByIdAsync(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("janesmith", result.Username);
            Assert.Equal("jane.smith@example.com", result.Email);
            Assert.Equal("Jane", result.FirstName);
            Assert.Equal("Smith", result.LastName);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((User)null);

            // Act
            var result = await _mockRepository.Object.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithValidUsername_ReturnsUser()
        {
            // Arrange
            var expectedUser = _testUsers.First(u => u.Username == "johndoe");
            _mockRepository.Setup(repo => repo.GetByUsernameAsync("johndoe"))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _mockRepository.Object.GetByUsernameAsync("johndoe");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("johndoe", result.Username);
            Assert.Equal("john.doe@example.com", result.Email);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithInvalidUsername_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByUsernameAsync("nonexistent"))
                .ReturnsAsync((User)null);

            // Act
            var result = await _mockRepository.Object.GetByUsernameAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
        {
            // Arrange
            var expectedUser = _testUsers.First(u => u.Email == "bob.williams@example.com");
            _mockRepository.Setup(repo => repo.GetByEmailAsync("bob.williams@example.com"))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _mockRepository.Object.GetByEmailAsync("bob.williams@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal("bobwilliams", result.Username);
            Assert.Equal("bob.williams@example.com", result.Email);
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByEmailAsync("nonexistent@example.com"))
                .ReturnsAsync((User)null);

            // Act
            var result = await _mockRepository.Object.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetActiveUsersAsync_ReturnsOnlyActiveUsers()
        {
            // Arrange
            var activeUsers = _testUsers.Where(u => u.IsActive).ToList();
            _mockRepository.Setup(repo => repo.GetActiveUsersAsync())
                .ReturnsAsync(activeUsers);

            // Act
            var result = await _mockRepository.Object.GetActiveUsersAsync();

            // Assert
            var users = result.ToList();
            Assert.Equal(2, users.Count);
            Assert.All(users, user => Assert.True(user.IsActive));
            Assert.Contains(users, u => u.Id == 1);
            Assert.Contains(users, u => u.Id == 2);
            Assert.DoesNotContain(users, u => u.Id == 3);
        }

        [Fact]
        public async Task AddAsync_AddsUserAndReturnsWithId()
        {
            // Arrange
            var newUser = new User
            {
                Username = "newuser",
                Email = "new.user@example.com",
                FirstName = "New",
                LastName = "User",
                IsActive = true
            };
            
            var addedUser = new User
            {
                Id = 4,
                Username = "newuser",
                Email = "new.user@example.com",
                FirstName = "New",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(addedUser);

            // Act
            var result = await _mockRepository.Object.AddAsync(newUser);

            // Assert
            Assert.Equal(4, result.Id);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("new.user@example.com", result.Email);
            Assert.Equal("New", result.FirstName);
            Assert.Equal("User", result.LastName);
            Assert.True(result.IsActive);
            Assert.NotEqual(default, result.CreatedAt);
            Assert.NotEqual(default, result.UpdatedAt);
        }

        [Fact]
        public async Task UpdateAsync_WithValidUser_UpdatesAndReturnsTrue()
        {
            // Arrange
            var userToUpdate = new User
            {
                Id = 1,
                Username = "johndoe_updated",
                Email = "john.updated@example.com",
                FirstName = "John Updated",
                LastName = "Doe Updated",
                IsActive = true
            };
            
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            var result = await _mockRepository.Object.UpdateAsync(userToUpdate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var userToUpdate = new User
            {
                Id = 999,
                Username = "invalid",
                Email = "invalid@example.com",
                FirstName = "Invalid",
                LastName = "User"
            };
            
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(false);

            // Act
            var result = await _mockRepository.Object.UpdateAsync(userToUpdate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesAndReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(3))
                .ReturnsAsync(true);

            // Act
            var result = await _mockRepository.Object.DeleteAsync(3);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _mockRepository.Object.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ActivateUserAsync_WithValidId_UpdatesAndReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ActivateUserAsync(3))
                .ReturnsAsync(true);

            // Act
            var result = await _mockRepository.Object.ActivateUserAsync(3);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeactivateUserAsync_WithValidId_UpdatesAndReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeactivateUserAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _mockRepository.Object.DeactivateUserAsync(1);

            // Assert
            Assert.True(result);
        }
    }
} 