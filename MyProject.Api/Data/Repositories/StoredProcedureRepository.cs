using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Microsoft.Extensions.Logging;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Base repository with stored procedure support
    /// </summary>
    public abstract class StoredProcedureRepository
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly string? _connectionString;
        protected readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the StoredProcedureRepository class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <param name="logger">The logger</param>
        protected StoredProcedureRepository(ApplicationDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetConnectionString();
            
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is null or empty.");
            }
        }

        /// <summary>
        /// Executes a stored procedure with error handling and logging
        /// </summary>
        /// <typeparam name="TResult">The type of result expected</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <param name="executeFunction">The function to execute the stored procedure</param>
        /// <param name="fallbackFunction">The fallback function to execute if the stored procedure fails</param>
        /// <returns>The result of the stored procedure or fallback</returns>
        protected async Task<TResult> ExecuteWithLoggingAsync<TResult>(
            string storedProcedure, 
            object? parameters, 
            Func<string, object?, Task<TResult>> executeFunction,
            Func<Exception, Task<TResult>> fallbackFunction)
        {
            try
            {
                return await executeFunction(storedProcedure, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", storedProcedure);
                return await fallbackFunction(ex);
            }
        }

        /// <summary>
        /// Logs the result of a stored procedure execution
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="result">The result returned by the stored procedure</param>
        protected void LogStoredProcedureResult(string storedProcedure, int result)
        {
            _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", storedProcedure, result);
        }

        /// <summary>
        /// Executes a stored procedure that returns a single result set asynchronously
        /// </summary>
        /// <typeparam name="T">The type of results to return</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>A collection of results</returns>
        protected async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Executes a stored procedure that returns a single entity asynchronously
        /// </summary>
        /// <typeparam name="T">The type of result to return</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>A single entity or default value if not found</returns>
        protected async Task<T?> ExecuteStoredProcedureFirstOrDefaultAsync<T>(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Executes a stored procedure that returns a single value asynchronously
        /// </summary>
        /// <typeparam name="T">The type of result to return</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>A single value</returns>
        protected async Task<T> ExecuteStoredProcedureScalarAsync<T>(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var result = await connection.ExecuteScalarAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            
            if (result == null)
            {
                throw new InvalidOperationException($"Stored procedure {storedProcedure} returned null when a value of type {typeof(T).Name} was expected.");
            }
            
            return result;
        }

        /// <summary>
        /// Executes a stored procedure that performs an action and returns the number of affected rows asynchronously
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>The number of rows affected</returns>
        protected async Task<int> ExecuteStoredProcedureNonQueryAsync(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Executes a stored procedure that returns a status code asynchronously
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>The stored procedure return value (status code)</returns>
        protected async Task<int> ExecuteStoredProcedureWithReturnValueAsync(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            // Add parameters if any
            if (parameters != null)
            {
                var parameterProperties = parameters.GetType().GetProperties();
                foreach (var prop in parameterProperties)
                {
                    var value = prop.GetValue(parameters);
                    command.Parameters.AddWithValue($"@{prop.Name}", value ?? DBNull.Value);
                }
            }
            
            // Add return value parameter
            var returnParam = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
            returnParam.Direction = ParameterDirection.ReturnValue;
            
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            
            var result = (int)returnParam.Value;
            LogStoredProcedureResult(storedProcedure, result);
            return result;
        }
    }
} 