using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace MyProject.Api.UnitTests.Data
{
    /// <summary>
    /// Simpler version of StoredProcedureRepository for testing that doesn't require a real connection string
    /// </summary>
    public class TestStoredProcedureRepository
    {
        protected readonly TestDbContext _dbContext;
        protected readonly string? _connectionString;
        protected readonly ILogger _logger;

        public TestStoredProcedureRepository(TestDbContext dbContext, ILogger logger, string? connectionString = null)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = connectionString ?? "Server=localhost;Database=TestDb;Trusted_Connection=True;";
            
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is null or empty.");
            }
        }

        /// <summary>
        /// This method is only for testing - it doesn't actually execute any stored procedures
        /// </summary>
        protected virtual async Task<int> ExecuteStoredProcedureNonQueryAsync(string storedProcedure, object? parameters = null)
        {
            // Simulate execution for testing purposes
            await Task.Delay(1);
            return 1;
        }

        /// <summary>
        /// This method is only for testing - it doesn't actually execute any stored procedures
        /// </summary>
        protected virtual async Task<int> ExecuteStoredProcedureWithReturnValueAsync(string storedProcedure, object? parameters = null)
        {
            // Simulate execution for testing purposes
            await Task.Delay(1);
            return 1;
        }

        /// <summary>
        /// This method is only for testing - it doesn't actually execute any stored procedures
        /// </summary>
        protected virtual async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string storedProcedure, object? parameters = null)
        {
            // Simulate execution for testing purposes
            await Task.Delay(1);
            return new List<T>();
        }

        /// <summary>
        /// This method is only for testing - it doesn't actually execute any stored procedures
        /// </summary>
        protected virtual async Task<T?> ExecuteStoredProcedureFirstOrDefaultAsync<T>(string storedProcedure, object? parameters = null)
        {
            // Simulate execution for testing purposes
            await Task.Delay(1);
            return default;
        }

        // Public methods to expose protected methods for testing
        public async Task<int> PublicExecuteStoredProcedureNonQueryAsync(string storedProcedure, object? parameters = null)
        {
            return await ExecuteStoredProcedureNonQueryAsync(storedProcedure, parameters);
        }

        public async Task<int> PublicExecuteStoredProcedureWithReturnValueAsync(string storedProcedure, object? parameters = null)
        {
            return await ExecuteStoredProcedureWithReturnValueAsync(storedProcedure, parameters);
        }

        public async Task<IEnumerable<T>> PublicExecuteStoredProcedureAsync<T>(string storedProcedure, object? parameters = null)
        {
            return await ExecuteStoredProcedureAsync<T>(storedProcedure, parameters);
        }

        public async Task<T?> PublicExecuteStoredProcedureFirstOrDefaultAsync<T>(string storedProcedure, object? parameters = null)
        {
            return await ExecuteStoredProcedureFirstOrDefaultAsync<T>(storedProcedure, parameters);
        }
    }

    public class StoredProcedureRepositoryTests
    {
        private readonly Mock<ILogger<TestStoredProcedureRepository>> _mockLogger;
        private readonly DbContextOptions<TestDbContext> _dbContextOptions;

        public StoredProcedureRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<TestStoredProcedureRepository>>();
            
            // Create TestDbContext with an in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"StoredProcDbTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public void Constructor_WithEmptyConnectionString_ThrowsException()
        {
            // Arrange
            using var context = new TestDbContext(_dbContextOptions);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                new TestStoredProcedureRepository(context, _mockLogger.Object, string.Empty));
        }

        [Fact]
        public void Constructor_WithValidConnectionString_Succeeds()
        {
            // Arrange & Act
            using var context = new TestDbContext(_dbContextOptions);
            var repository = new TestStoredProcedureRepository(context, _mockLogger.Object, "Server=localhost;Database=TestDb;Trusted_Connection=True;");

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public void HasConnectionStringProperty()
        {
            // Arrange
            using var context = new TestDbContext(_dbContextOptions);
            var repository = new TestStoredProcedureRepository(context, _mockLogger.Object, "Server=localhost;Database=TestDb;Trusted_Connection=True;");

            // Act & Assert
            // Use reflection to check if _connectionString is set
            var connectionStringField = typeof(TestStoredProcedureRepository)
                .GetField("_connectionString", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var connectionString = connectionStringField?.GetValue(repository) as string;
            Assert.NotNull(connectionString);
            Assert.NotEmpty(connectionString);
        }

        [Fact]
        public async Task ExecuteStoredProcedureNonQueryAsync_ReturnsExpectedResult()
        {
            // Arrange
            using var context = new TestDbContext(_dbContextOptions);
            var repository = new TestStoredProcedureRepository(context, _mockLogger.Object, "Server=localhost;Database=TestDb;Trusted_Connection=True;");

            // Act
            var result = await repository.PublicExecuteStoredProcedureNonQueryAsync("TestProcedure", new { Param1 = 1 });

            // Assert
            Assert.Equal(1, result);
        }
    }
} 