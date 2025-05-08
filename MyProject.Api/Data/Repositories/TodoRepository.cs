using Microsoft.EntityFrameworkCore;
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
        public TodoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<Todo>> GetAllAsync()
        {
            try
            {
                // Try to use stored procedure for better performance
                return await ExecuteStoredProcedureAsync<Todo>(SP_GET_ALL_TODOS);
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                return await _dbSet.OrderByDescending(t => t.CreatedAt).ToListAsync();
            }
        }
        
        /// <inheritdoc />
        public override async Task<Todo?> GetByIdAsync(int id)
        {
            try
            {
                // Try to use stored procedure for better performance
                return await ExecuteStoredProcedureFirstOrDefaultAsync<Todo>(SP_GET_TODO_BY_ID, new { Id = id });
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                return await _dbSet.FindAsync(id);
            }
        }
        
        /// <inheritdoc />
        public override async Task<bool> DeleteAsync(int id)
        {
            try
            {
                // Try to use stored procedure for better performance
                return await ExecuteStoredProcedureNonQueryAsync(SP_DELETE_TODO, new { Id = id }) > 0;
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                var todo = await _dbSet.FindAsync(id);
                if (todo == null)
                    return false;
                    
                _dbSet.Remove(todo);
                return await _dbContext.SaveChangesAsync() > 0;
            }
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetCompletedAsync()
        {
            try
            {
                // Try to use stored procedure for better performance
                return await ExecuteStoredProcedureAsync<Todo>(SP_GET_COMPLETED_TODOS);
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                return await _dbSet.Where(t => t.IsCompleted).OrderByDescending(t => t.UpdatedAt).ToListAsync();
            }
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<Todo>> GetIncompleteAsync()
        {
            try
            {
                // Try to use stored procedure for better performance
                return await ExecuteStoredProcedureAsync<Todo>(SP_GET_INCOMPLETE_TODOS);
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                return await _dbSet.Where(t => !t.IsCompleted).OrderByDescending(t => t.CreatedAt).ToListAsync();
            }
        }
        
        /// <inheritdoc />
        public async Task<bool> MarkAsCompletedAsync(int id)
        {
            try
            {
                // Try to use stored procedure for better performance
                return await ExecuteStoredProcedureNonQueryAsync(SP_MARK_TODO_COMPLETED, new { Id = id }) > 0;
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
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
                // Try to use stored procedure for better performance
                return await ExecuteStoredProcedureNonQueryAsync(SP_MARK_TODO_INCOMPLETE, new { Id = id }) > 0;
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
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
            // Use the stored procedure for adding a todo
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            
            try
            {
                // Using stored procedure to add
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
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                await _dbSet.AddAsync(todo);
                await _dbContext.SaveChangesAsync();
                return todo;
            }
        }
        
        /// <inheritdoc />
        public override async Task<bool> UpdateAsync(Todo todo)
        {
            try
            {
                // Use stored procedure to update the todo
                return await ExecuteStoredProcedureNonQueryAsync(SP_UPDATE_TODO, new
                {
                    todo.Id,
                    todo.Title,
                    todo.Description,
                    todo.IsCompleted,
                    UpdatedAt = DateTime.UtcNow
                }) > 0;
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                var existingTodo = await _dbSet.FindAsync(todo.Id);
                if (existingTodo == null)
                    return false;
                    
                existingTodo.Title = todo.Title;
                existingTodo.Description = todo.Description;
                existingTodo.IsCompleted = todo.IsCompleted;
                existingTodo.UpdatedAt = DateTime.UtcNow;
                
                return await _dbContext.SaveChangesAsync() > 0;
            }
        }
        
        /// <inheritdoc />
        public override IEnumerable<Todo> GetAll()
        {
            // Use stored procedure for better performance
            return ExecuteStoredProcedure<Todo>(SP_GET_ALL_TODOS);
            
            // Fallback to EF Core if needed
            // return _dbSet.OrderByDescending(t => t.CreatedAt).ToList();
        }
        
        /// <inheritdoc />
        public override Todo? GetById(int id)
        {
            // Use stored procedure for better performance
            return ExecuteStoredProcedureFirstOrDefault<Todo>(SP_GET_TODO_BY_ID, new { Id = id });
            
            // Fallback to EF Core if needed
            // return _dbSet.Find(id);
        }
        
        /// <inheritdoc />
        public override bool Delete(int id)
        {
            // Use stored procedure for better performance
            return ExecuteStoredProcedureNonQuery(SP_DELETE_TODO, new { Id = id }) > 0;
            
            // Fallback to EF Core if needed
            /*
            var todo = _dbSet.Find(id);
            if (todo == null)
                return false;
                
            _dbSet.Remove(todo);
            return _dbContext.SaveChanges() > 0;
            */
        }
        
        /// <inheritdoc />
        public IEnumerable<Todo> GetCompleted()
        {
            // Check if the stored procedure exists in the database
            try
            {
                return ExecuteStoredProcedure<Todo>(SP_GET_COMPLETED_TODOS);
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                return _dbSet.Where(t => t.IsCompleted).OrderByDescending(t => t.UpdatedAt).ToList();
            }
        }
        
        /// <inheritdoc />
        public IEnumerable<Todo> GetIncomplete()
        {
            // Check if the stored procedure exists in the database
            try
            {
                return ExecuteStoredProcedure<Todo>(SP_GET_INCOMPLETE_TODOS);
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
                return _dbSet.Where(t => !t.IsCompleted).OrderByDescending(t => t.CreatedAt).ToList();
            }
        }
        
        /// <inheritdoc />
        public bool MarkAsCompleted(int id)
        {
            // Check if the stored procedure exists in the database
            try
            {
                return ExecuteStoredProcedureNonQuery(SP_MARK_TODO_COMPLETED, new { Id = id }) > 0;
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
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
            // Check if the stored procedure exists in the database
            try
            {
                return ExecuteStoredProcedureNonQuery(SP_MARK_TODO_INCOMPLETE, new { Id = id }) > 0;
            }
            catch
            {
                // Fallback to EF Core if the stored procedure doesn't exist
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
            // Use the stored procedure for adding a todo
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            
            // Using stored procedure to add
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
            
            // Fallback to EF Core if needed
            /*
            _dbSet.Add(todo);
            _dbContext.SaveChanges();
            return todo;
            */
        }
        
        /// <inheritdoc />
        public override bool Update(Todo todo)
        {
            // Use stored procedure to update the todo
            return ExecuteStoredProcedureNonQuery(SP_UPDATE_TODO, new
            {
                todo.Id,
                todo.Title,
                todo.Description,
                todo.IsCompleted,
                UpdatedAt = DateTime.UtcNow
            }) > 0;
            
            // Fallback to EF Core if needed
            /*
            var existingTodo = _dbSet.Find(todo.Id);
            if (existingTodo == null)
                return false;
                
            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.IsCompleted = todo.IsCompleted;
            existingTodo.UpdatedAt = DateTime.UtcNow;
            
            return _dbContext.SaveChanges() > 0;
            */
        }
    }
} 