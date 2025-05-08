using Microsoft.EntityFrameworkCore;
using MyProject.Api.Models;

namespace MyProject.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Todo>(entity =>
            {
                entity.ToTable("Todo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description);
                entity.Property(e => e.IsCompleted).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });
            
            // Seed data can be added here if needed
            // modelBuilder.Entity<Todo>().HasData(
            //     new Todo { Id = 1, Title = "First Todo", Description = "This is the first todo item", IsCompleted = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            // );
        }
        
        /// <summary>
        /// Ensures the database is created and migrated
        /// </summary>
        public void EnsureDatabaseCreated()
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Seeds the database with initial data
        /// </summary>
        public void SeedData()
        {
            if (!Todos.Any())
            {
                Todos.AddRange(
                    new Todo 
                    { 
                        Title = "Learn ASP.NET Core", 
                        Description = "Study ASP.NET Core web API development",
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Todo 
                    { 
                        Title = "Build Docker containers", 
                        Description = "Learn how to containerize .NET applications",
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Todo 
                    { 
                        Title = "Implement CI/CD", 
                        Description = "Set up continuous integration and deployment",
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                );
                
                SaveChanges();
            }
        }
    }
} 