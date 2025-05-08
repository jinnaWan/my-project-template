using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyProject.Api.Models;

namespace MyProject.Api.Data
{
    /// <summary>
    /// SQL Server implementation of ITodoRepository that uses Entity Framework Core
    /// while supporting direct SQL execution for stored procedures when needed
    /// </summary>
    public class SqlTodoRepository : ITodoRepository
    {
        private readonly ApplicationDbContext _dbContext;
        
        /// <summary>
        /// Initializes a new instance of the SqlTodoRepository class
        /// </summary>
        /// <param name="dbContext">The database context</param>
        public SqlTodoRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public IEnumerable<Todo> GetAll()
        {
            return _dbContext.Todos.OrderByDescending(t => t.CreatedAt).ToList();
        }

        /// <inheritdoc />
        public Todo? GetById(int id)
        {
            return _dbContext.Todos.Find(id);
        }

        /// <inheritdoc />
        public Todo Add(Todo todo)
        {
            todo.CreatedAt = DateTime.UtcNow;
            todo.UpdatedAt = DateTime.UtcNow;
            
            _dbContext.Todos.Add(todo);
            _dbContext.SaveChanges();
            
            return todo;
        }

        /// <inheritdoc />
        public bool Update(Todo todo)
        {
            var existingTodo = _dbContext.Todos.Find(todo.Id);
            if (existingTodo == null)
                return false;
                
            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.IsCompleted = todo.IsCompleted;
            existingTodo.UpdatedAt = DateTime.UtcNow;
            
            return _dbContext.SaveChanges() > 0;
        }

        /// <inheritdoc />
        public bool Delete(int id)
        {
            var todo = _dbContext.Todos.Find(id);
            if (todo == null)
                return false;
                
            _dbContext.Todos.Remove(todo);
            return _dbContext.SaveChanges() > 0;
        }
        
        // Example of using stored procedures directly if needed
        // public IEnumerable<Todo> GetAllUsingStoredProcedure()
        // {
        //     return _dbContext.Todos.FromSqlRaw("EXEC sp_GetAllTodos").ToList();
        // }
    }
} 