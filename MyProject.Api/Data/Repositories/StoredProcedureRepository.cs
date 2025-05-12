using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Base repository with stored procedure support
    /// </summary>
    public abstract class StoredProcedureRepository
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly string? _connectionString;

        /// <summary>
        /// Initializes a new instance of the StoredProcedureRepository class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        protected StoredProcedureRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _connectionString = _dbContext.Database.GetConnectionString();
            
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string is null or empty.");
            }
        }

        /// <summary>
        /// Executes a stored procedure that returns a single result set
        /// </summary>
        /// <typeparam name="T">The type of results to return</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>A collection of results</returns>
        protected IEnumerable<T> ExecuteStoredProcedure<T>(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Executes a stored procedure that returns a single entity
        /// </summary>
        /// <typeparam name="T">The type of result to return</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>A single entity or default value if not found</returns>
        protected T? ExecuteStoredProcedureFirstOrDefault<T>(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.QueryFirstOrDefault<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Executes a stored procedure that returns a single value
        /// </summary>
        /// <typeparam name="T">The type of result to return</typeparam>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>A single value</returns>
        protected T ExecuteStoredProcedureScalar<T>(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = connection.ExecuteScalar<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            
            if (result == null)
            {
                throw new InvalidOperationException($"Stored procedure {storedProcedure} returned null when a value of type {typeof(T).Name} was expected.");
            }
            
            return result;
        }

        /// <summary>
        /// Executes a stored procedure that performs an action and returns the number of affected rows
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>The number of rows affected</returns>
        protected int ExecuteStoredProcedureNonQuery(string storedProcedure, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
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
        /// Executes a stored procedure that returns a status code
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure</param>
        /// <param name="parameters">The parameters to pass to the stored procedure</param>
        /// <returns>The stored procedure return value (status code)</returns>
        protected int ExecuteStoredProcedureWithReturnValue(string storedProcedure, object? parameters = null)
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
            
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            
            return (int)returnParam.Value;
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
            
            return (int)returnParam.Value;
        }
    }
} 