# Todo API with SQL Server Database

A simple Todo API with ASP.NET Core and SQL Server running in Docker.

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Azure Data Studio](https://learn.microsoft.com/en-us/azure-data-studio/download-azure-data-studio) (for database management)

## Quick Start

```bash
# Start the database, build and run the application
make all
```

This will:
1. Start SQL Server in a Docker container
2. Build the application
3. Run the application, which will create and seed the database automatically

The API will be available at http://localhost:5000 or https://localhost:5001

## Database Management

This project uses SQL Server for data storage. We recommend using Azure Data Studio for database management.

### Azure Data Studio

1. Download and install [Azure Data Studio](https://learn.microsoft.com/en-us/azure-data-studio/download-azure-data-studio)
2. Connect to the database using:
   - Server: `localhost,1433`
   - Authentication: SQL Login
   - Username: `sa`
   - Password: `YourStrong@Passw0rd`
   - Database: `TodoDb` (after application first run)

![Azure Data Studio Connection](https://learn.microsoft.com/en-us/azure-data-studio/media/quickstart-sql-server/connect-screen.png)

### Connection String

The application uses the following connection string:
```
Server=localhost,1433;Database=TodoDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

## Available Commands

Run `make help` to see all available commands:

- `make start-db` - Start SQL Server container
- `make stop-db` - Stop and remove containers
- `make build` - Build the application
- `make run` - Run the application (creates and seeds DB automatically)
- `make watch` - Run the application with hot reload
- `make test` - Run all tests
- `make clean` - Clean build artifacts
- `make all` - Start DB, build and run the application

## Project Structure

- `MyProject.Api/` - The API project
  - `Controllers/` - API controllers
  - `Models/` - Data models
  - `Data/` - Database context and repositories
  - `Services/` - Business logic services

## Architecture Overview

This project implements a clean architecture pattern:
- **API Layer**: Controllers that handle HTTP requests and responses
- **Service Layer**: Business logic and validation
- **Repository Layer**: Data access using Entity Framework Core
- **Entity Framework Core**: ORM for database interactions
- **Docker Containers**: For easy deployment and development

Database schema is created using Entity Framework Core's code-first approach, which automatically creates and seeds the database on application startup. 