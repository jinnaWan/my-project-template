using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Api.Models;
using System.Data.Common;

namespace MyProject.Api.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Todo entities using stored procedures for critical operations
    /// </summary>
    public class TodoRepository : BaseRepository<Todo>, ITodoRepository
    {
        // Stored procedure names
        private const string SP_GET_ALL_TODOS = "sp_GetAllTodos";
        private const string SP_GET_TODO_BY_ID = "sp_GetTodoById";
        private const string SP_CREATE_TODO = "sp_CreateTodo";
        private const string SP_UPDATE_TODO = "sp_UpdateTodo";
        private const string SP_DELETE_TODO = "sp_DeleteTodo";
        private const string SP_GET_COMPLETED_TODOS = "sp_GetCompletedTodos";
        private const string SP_GET_INCOMPLETE_TODOS = "sp_GetIncompleteTodos";
        private const string SP_MARK_TODO_COMPLETED = "sp_MarkTodoCompleted";
        private const string SP_MARK_TODO_INCOMPLETE = "sp_MarkTodoIncomplete";
        
        private readonly ILogger<TodoRepository> _logger;
        
        /// <summary>
        /// Initializes a new instance of the TodoRepository class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <param name="logger">The logger</param>
        public TodoRepository(ApplicationDbContext dbContext, ILogger<TodoRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<Todo>> GetAllAsync()
        {
            try
            {
                return await ExecuteStoredProcedureAsync<Todo>(SP_GET_ALL_TODOS);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_GET_ALL_TODOS);
                return await _dbSet.OrderByDescending(t => t.CreatedAt).ToListAsync();
            }
        }
        
        /// <inheritdoc />
        public override async Task<Todo?> GetByIdAsync(int id)
        {
            try
            {
                return await ExecuteStoredProcedureFirstOrDefaultAsync<Todo>(SP_GET_TODO_BY_ID, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_GET_TODO_BY_ID);
                return await _dbSet.FindAsync(id);
            }
        }
        
        /// <inheritdoc />
        public override async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to delete Todo with ID {TodoId}", id);
            
            try
            {
                var result = await ExecuteStoredProcedureWithReturnValueAsync(SP_DELETE_TODO, new { Id = id });
                _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", SP_DELETE_TODO, result);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_DELETE_TODO);
                
                var todo = await _dbSet.FindAsync(id);
                if (todo == null)
                {
                    _logger.LogWarning("Todo with ID {TodoId} not found in the database", id);
                    return false;
                }
                
                _dbSet.Remove(todo);
                var result = await _dbContext.SaveChangesAsync() > 0;
                _logger.LogInformation("EF Core delete result: {Result}", result);
                return result;
            }
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetCompletedAsync()
        {
            try
            {
                return await ExecuteStoredProcedureAsync<Todo>(SP_GET_COMPLETED_TODOS);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_GET_COMPLETED_TODOS);
                return await _dbSet.Where(t => t.IsCompleted).OrderByDescending(t => t.UpdatedAt).ToListAsync();
            }
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetIncompleteAsync()
        {
            try
            {
                return await ExecuteStoredProcedureAsync<Todo>(SP_GET_INCOMPLETE_TODOS);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_GET_INCOMPLETE_TODOS);
                return await _dbSet.Where(t => !t.IsCompleted).OrderByDescending(t => t.CreatedAt).ToListAsync();
            }
        }
        
        /// <inheritdoc />
        public async Task<bool> MarkAsCompletedAsync(int id)
        {
            try
            {
                var result = await ExecuteStoredProcedureWithReturnValueAsync(SP_MARK_TODO_COMPLETED, new { Id = id });
                _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", SP_MARK_TODO_COMPLETED, result);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_MARK_TODO_COMPLETED);
                
                var todo = await _dbSet.FindAsync(id);
                if (todo == null)
                    return false;
                    
                todo.IsCompleted = true;
                todo.UpdatedAt = DateTime.UtcNow;
                
                return await _dbContext.SaveChangesAsync() > 0;
            }
        }
        
        /// <inheritdoc />
        public async Task<bool> MarkAsIncompleteAsync(int id)
        {
            try
            {
                var result = await ExecuteStoredProcedureWithReturnValueAsync(SP_MARK_TODO_INCOMPLETE, new { Id = id });
                _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", SP_MARK_TODO_INCOMPLETE, result);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_MARK_TODO_INCOMPLETE);
                
                var todo = await _dbSet.FindAsync(id);
                if (todo == null)
                    return false;
                    
                todo.IsCompleted = false;
                todo.UpdatedAt = DateTime.UtcNow;
                
                return await _dbContext.SaveChangesAsync() > 0;
            }
        }
        
        /// <inheritdoc />
        public override async Task<Todo> AddAsync(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            
            try
            {
                var id = await ExecuteStoredProcedureScalarAsync<int>(SP_CREATE_TODO, new 
                {
                    todo.Title,
                    todo.Description,
                    todo.IsCompleted,
                    todo.CreatedAt,
                    todo.UpdatedAt
                });
                
                todo.Id = id;
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_CREATE_TODO);
                
                await _dbSet.AddAsync(todo);
                await _dbContext.SaveChangesAsync();
                return todo;
            }
        }
        
        /// <inheritdoc />
        public override async Task<bool> UpdateAsync(Todo todo)
        {
            _logger.LogInformation("Attempting to update Todo with ID {TodoId}", todo.Id);
            
            try
            {
                // First check if the Todo exists
                var existingTodo = await _dbSet.FindAsync(todo.Id);
                if (existingTodo == null)
                {
                    _logger.LogWarning("Todo with ID {TodoId} not found in database before attempting update", todo.Id);
                    return false;
                }
                
                todo.UpdatedAt = DateTime.UtcNow;
                
                var result = await ExecuteStoredProcedureWithReturnValueAsync(SP_UPDATE_TODO, new
                {
                    todo.Id,
                    todo.Title,
                    todo.Description,
                    todo.IsCompleted,
                    UpdatedAt = todo.UpdatedAt
                });
                
                _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", SP_UPDATE_TODO, result);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_UPDATE_TODO);
                
                var existingTodo = await _dbSet.FindAsync(todo.Id);
                if (existingTodo == null)
                {
                    _logger.LogWarning("Todo with ID {TodoId} not found in the database", todo.Id);
                    return false;
                }
                
                existingTodo.Title = todo.Title;
                existingTodo.Description = todo.Description;
                existingTodo.IsCompleted = todo.IsCompleted;
                existingTodo.UpdatedAt = DateTime.UtcNow;
                
                var result = await _dbContext.SaveChangesAsync() > 0;
                _logger.LogInformation("EF Core update result: {Result}", result);
                return result;
            }
        }
        
        /// <inheritdoc />
        public override IEnumerable<Todo> GetAll()
        {
            try
            {
                return ExecuteStoredProcedure<Todo>(SP_GET_ALL_TODOS);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_GET_ALL_TODOS);
                return _dbSet.OrderByDescending(t => t.CreatedAt).ToList();
            }
        }
        
        /// <inheritdoc />
        public override Todo? GetById(int id)
        {
            try
            {
                return ExecuteStoredProcedureFirstOrDefault<Todo>(SP_GET_TODO_BY_ID, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_GET_TODO_BY_ID);
                return _dbSet.Find(id);
            }
        }
        
        /// <inheritdoc />
        public override bool Delete(int id)
        {
            _logger.LogInformation("Attempting to delete Todo with ID {TodoId} (synchronous method)", id);
            
            try
            {
                var result = ExecuteStoredProcedureWithReturnValue(SP_DELETE_TODO, new { Id = id });
                _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", SP_DELETE_TODO, result);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_DELETE_TODO);
                
                var todo = _dbSet.Find(id);
                if (todo == null)
                {
                    _logger.LogWarning("Todo with ID {TodoId} not found in the database", id);
                    return false;
                }
                
                _dbSet.Remove(todo);
                var result = _dbContext.SaveChanges() > 0;
                _logger.LogInformation("EF Core delete result: {Result}", result);
                return result;
            }
        }
        
        /// <inheritdoc />
        public IEnumerable<Todo> GetCompleted()
        {
            try
            {
                return ExecuteStoredProcedure<Todo>(SP_GET_COMPLETED_TODOS);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_GET_COMPLETED_TODOS);
                return _dbSet.Where(t => t.IsCompleted).OrderByDescending(t => t.UpdatedAt).ToList();
            }
        }
        
        /// <inheritdoc />
        public IEnumerable<Todo> GetIncomplete()
        {
            try
            {
                return ExecuteStoredProcedure<Todo>(SP_GET_INCOMPLETE_TODOS);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_GET_INCOMPLETE_TODOS);
                return _dbSet.Where(t => !t.IsCompleted).OrderByDescending(t => t.CreatedAt).ToList();
            }
        }
        
        /// <inheritdoc />
        public bool MarkAsCompleted(int id)
        {
            try
            {
                var result = ExecuteStoredProcedureWithReturnValue(SP_MARK_TODO_COMPLETED, new { Id = id });
                _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", SP_MARK_TODO_COMPLETED, result);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_MARK_TODO_COMPLETED);
                
                var todo = _dbSet.Find(id);
                if (todo == null)
                    return false;
                    
                todo.IsCompleted = true;
                todo.UpdatedAt = DateTime.UtcNow;
                
                return _dbContext.SaveChanges() > 0;
            }
        }
        
        /// <inheritdoc />
        public bool MarkAsIncomplete(int id)
        {
            try
            {
                var result = ExecuteStoredProcedureWithReturnValue(SP_MARK_TODO_INCOMPLETE, new { Id = id });
                _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", SP_MARK_TODO_INCOMPLETE, result);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_MARK_TODO_INCOMPLETE);
                
                var todo = _dbSet.Find(id);
                if (todo == null)
                    return false;
                    
                todo.IsCompleted = false;
                todo.UpdatedAt = DateTime.UtcNow;
                
                return _dbContext.SaveChanges() > 0;
            }
        }
        
        /// <inheritdoc />
        public override Todo Add(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            
            try
            {
                var id = ExecuteStoredProcedureScalar<int>(SP_CREATE_TODO, new 
                {
                    todo.Title,
                    todo.Description,
                    todo.IsCompleted,
                    todo.CreatedAt,
                    todo.UpdatedAt
                });
                
                todo.Id = id;
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_CREATE_TODO);
                
                _dbSet.Add(todo);
                _dbContext.SaveChanges();
                return todo;
            }
        }
        
        /// <inheritdoc />
        public override bool Update(Todo todo)
        {
            _logger.LogInformation("Attempting to update Todo with ID {TodoId} (synchronous method)", todo.Id);
            
            try
            {
                // First check if the Todo exists
                var existingTodo = _dbSet.Find(todo.Id);
                if (existingTodo == null)
                {
                    _logger.LogWarning("Todo with ID {TodoId} not found in database before attempting update", todo.Id);
                    return false;
                }
                
                todo.UpdatedAt = DateTime.UtcNow;
                
                var result = ExecuteStoredProcedureWithReturnValue(SP_UPDATE_TODO, new
                {
                    todo.Id,
                    todo.Title,
                    todo.Description,
                    todo.IsCompleted,
                    UpdatedAt = todo.UpdatedAt
                });
                
                _logger.LogInformation("StoredProcedure {ProcName} returned status code {Result}", SP_UPDATE_TODO, result);
                
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to execute stored procedure {StoredProcedure}. Falling back to EF Core.", SP_UPDATE_TODO);
                
                var existingTodo = _dbSet.Find(todo.Id);
                if (existingTodo == null)
                {
                    _logger.LogWarning("Todo with ID {TodoId} not found in the database", todo.Id);
                    return false;
                }
                
                existingTodo.Title = todo.Title;
                existingTodo.Description = todo.Description;
                existingTodo.IsCompleted = todo.IsCompleted;
                existingTodo.UpdatedAt = DateTime.UtcNow;
                
                var result = _dbContext.SaveChanges() > 0;
                _logger.LogInformation("EF Core update result: {Result}", result);
                return result;
            }
        }
    }
} 