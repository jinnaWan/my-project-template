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
        public DbSet<User> Users { get; set; }

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
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("AppUser");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                
                // Create unique constraint on Username and Email
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
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
        /// Forces database recreation (deletes and recreates)
        /// </summary>
        public void RecreateDatabase()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        /// <summary>
        /// Seeds the database with initial data
        /// </summary>
        public void SeedData()
        {
            // Seed Todos
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
            
            // Seed Users
            if (!Users.Any())
            {
                Users.AddRange(
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        FirstName = "Admin",
                        LastName = "User",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "user1",
                        Email = "user1@example.com",
                        FirstName = "Regular",
                        LastName = "User",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                );
                
                SaveChanges();
            }
        }
    }
} 