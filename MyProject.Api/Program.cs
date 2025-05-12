using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyProject.Api.Data;
using MyProject.Api.Data.Repositories;
using MyProject.Api.Services;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyProject API",
        Version = "v1",
        Description = "API for the MyProject application"
    });
});

// Register application services
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddSingleton<IWeatherForecastService, WeatherForecastService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Initialize and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Force recreation of the database
        dbContext.RecreateDatabase();
        
        // Seed initial data
        dbContext.SeedData();
        
        // Execute stored procedure scripts
        ExecuteStoredProcedureScripts(dbContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyProject API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Executes all stored procedure SQL scripts
/// </summary>
void ExecuteStoredProcedureScripts(ApplicationDbContext dbContext, ILogger logger)
{
    try
    {
        // Look for SQL files in the new location (inside the API project)
        var scriptsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SqlScripts", "StoredProcedures");
        
        if (!Directory.Exists(scriptsDirectory))
        {
            logger.LogWarning("Stored procedures directory not found: {Directory}", scriptsDirectory);
            return;
        }
        
        // Get all SQL files recursively
        var sqlFiles = Directory.GetFiles(scriptsDirectory, "*.sql", SearchOption.AllDirectories);
        
        if (sqlFiles.Length == 0)
        {
            logger.LogWarning("No SQL files found in: {Directory}", scriptsDirectory);
            return;
        }
        
        logger.LogInformation("Found {Count} SQL files to execute", sqlFiles.Length);
        
        foreach (var sqlFile in sqlFiles)
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var scriptContent = File.ReadAllText(sqlFile);
                
                // Execute the script without GO statement (already removed from files)
                dbContext.Database.ExecuteSqlRaw(scriptContent);
                
                // Commit the transaction
                transaction.Commit();
                
                logger.LogInformation("Successfully executed SQL script: {FileName}", Path.GetFileName(sqlFile));
            }
            catch (Exception ex)
            {
                // Rollback on error
                transaction.Rollback();
                logger.LogError(ex, "Error executing SQL script {FileName}", Path.GetFileName(sqlFile));
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing stored procedure scripts");
    }
} 