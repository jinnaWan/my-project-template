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
        
        /// <summary>
        /// Initializes a new instance of the TodoRepository class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        /// <param name="logger">The logger</param>
        public TodoRepository(ApplicationDbContext dbContext, ILogger<TodoRepository> logger) : base(dbContext, logger)
        {
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<Todo>> GetAllAsync()
        {
            return await ExecuteWithLoggingAsync<IEnumerable<Todo>>(
                SP_GET_ALL_TODOS,
                null,
                ExecuteStoredProcedureAsync<Todo>,
                async (ex) => await _dbSet.OrderByDescending(t => t.CreatedAt).ToListAsync()
            );
        }
        
        /// <inheritdoc />
        public override async Task<Todo?> GetByIdAsync(int id)
        {
            return await ExecuteWithLoggingAsync<Todo?>(
                SP_GET_TODO_BY_ID,
                new { Id = id },
                ExecuteStoredProcedureFirstOrDefaultAsync<Todo>,
                async (ex) => await _dbSet.FindAsync(id)
            );
        }
        
        /// <inheritdoc />
        public override async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to delete Todo with ID {TodoId}", id);
            
            return await ExecuteWithLoggingAsync<bool>(
                SP_DELETE_TODO,
                new { Id = id },
                async (sp, p) => await ExecuteStoredProcedureWithReturnValueAsync(sp, p) > 0,
                async (ex) => {
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
            );
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetCompletedAsync()
        {
            return await ExecuteWithLoggingAsync<IEnumerable<Todo>>(
                SP_GET_COMPLETED_TODOS,
                null,
                ExecuteStoredProcedureAsync<Todo>,
                async (ex) => await _dbSet.Where(t => t.IsCompleted).OrderByDescending(t => t.UpdatedAt).ToListAsync()
            );
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetIncompleteAsync()
        {
            return await ExecuteWithLoggingAsync<IEnumerable<Todo>>(
                SP_GET_INCOMPLETE_TODOS,
                null,
                ExecuteStoredProcedureAsync<Todo>,
                async (ex) => await _dbSet.Where(t => !t.IsCompleted).OrderByDescending(t => t.CreatedAt).ToListAsync()
            );
        }
        
        /// <inheritdoc />
        public async Task<bool> MarkAsCompletedAsync(int id)
        {
            return await ExecuteWithLoggingAsync<bool>(
                SP_MARK_TODO_COMPLETED,
                new { Id = id },
                async (sp, p) => await ExecuteStoredProcedureWithReturnValueAsync(sp, p) > 0,
                async (ex) => {
                    var todo = await _dbSet.FindAsync(id);
                    if (todo == null)
                        return false;
                        
                    todo.IsCompleted = true;
                    todo.UpdatedAt = DateTime.UtcNow;
                    
                    return await _dbContext.SaveChangesAsync() > 0;
                }
            );
        }
        
        /// <inheritdoc />
        public async Task<bool> MarkAsIncompleteAsync(int id)
        {
            return await ExecuteWithLoggingAsync<bool>(
                SP_MARK_TODO_INCOMPLETE,
                new { Id = id },
                async (sp, p) => await ExecuteStoredProcedureWithReturnValueAsync(sp, p) > 0,
                async (ex) => {
                    var todo = await _dbSet.FindAsync(id);
                    if (todo == null)
                        return false;
                        
                    todo.IsCompleted = false;
                    todo.UpdatedAt = DateTime.UtcNow;
                    
                    return await _dbContext.SaveChangesAsync() > 0;
                }
            );
        }
        
        /// <inheritdoc />
        public override async Task<Todo> AddAsync(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            
            return await ExecuteWithLoggingAsync<Todo>(
                SP_CREATE_TODO,
                new {
                    todo.Title,
                    todo.Description,
                    todo.IsCompleted,
                    todo.CreatedAt,
                    todo.UpdatedAt
                },
                async (sp, p) => {
                    var id = await ExecuteStoredProcedureScalarAsync<int>(sp, p);
                    todo.Id = id;
                    return todo;
                },
                async (ex) => {
                    await _dbSet.AddAsync(todo);
                    await _dbContext.SaveChangesAsync();
                    return todo;
                }
            );
        }
        
        /// <inheritdoc />
        public override async Task<bool> UpdateAsync(Todo todo)
        {
            _logger.LogInformation("Attempting to update Todo with ID {TodoId}", todo.Id);
            
            // First check if the Todo exists
            var existingTodo = await _dbSet.FindAsync(todo.Id);
            if (existingTodo == null)
            {
                _logger.LogWarning("Todo with ID {TodoId} not found in database before attempting update", todo.Id);
                return false;
            }
            
            todo.UpdatedAt = DateTime.UtcNow;
            
            return await ExecuteWithLoggingAsync<bool>(
                SP_UPDATE_TODO,
                new {
                    todo.Id,
                    todo.Title,
                    todo.Description,
                    todo.IsCompleted,
                    UpdatedAt = todo.UpdatedAt
                },
                async (sp, p) => await ExecuteStoredProcedureWithReturnValueAsync(sp, p) > 0,
                async (ex) => {
                    existingTodo.Title = todo.Title;
                    existingTodo.Description = todo.Description;
                    existingTodo.IsCompleted = todo.IsCompleted;
                    existingTodo.UpdatedAt = DateTime.UtcNow;
                    
                    var result = await _dbContext.SaveChangesAsync() > 0;
                    _logger.LogInformation("EF Core update result: {Result}", result);
                    return result;
                }
            );
        }
    }
} 