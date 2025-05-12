# Todo App

A modern todo application with a React frontend and .NET backend.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v18 or higher)
- [Docker](https://www.docker.com/products/docker-desktop) (for SQL Server)
- Make utility

## Project Structure

- `MyProject.Api` - .NET backend API
- `MyProject.Client` - React frontend
- `docker-compose.yml` - Docker configuration for SQL Server

## Getting Started

This project uses `make` commands to simplify development tasks. Below are the available commands:

### Database Commands

```bash
# Start SQL Server in Docker
make start-db

# Stop SQL Server container
make stop-db
```

### Backend Commands

```bash
# Build the backend
make build

# Run the backend
make run

# Run the backend with hot reload
make watch

# Run all backend tests
make test

# Run only backend unit tests
make test-unit
```

### Frontend Commands

```bash
# Start frontend development server
make frontend-dev

# Build frontend for production
make frontend-build

# Run frontend tests
make frontend-test

# Run frontend linting
make frontend-lint
```

### Combined Commands

```bash
# Start both backend and frontend for development
make dev

# Start DB, build and run the application
make all
```

### Clean Build Artifacts

```bash
# Clean build artifacts for both backend and frontend
make clean
```

## Development Workflow

The easiest way to start development is:

1. Clone the repository
2. Run `make start-db` to start the SQL Server container
3. Run `make dev` to start both the backend and frontend in development mode

This will start:
- SQL Server on `localhost:1433`
- Backend API on `http://localhost:5000`
- Frontend on `http://localhost:5173`

## Database Connection

To connect to the SQL Server database using Azure Data Studio or SQL Server Management Studio:

- Server: `localhost,1433`
- Authentication: SQL Login
- Username: `sa`
- Password: `YourStrong@Passw0rd`
- Database: `TodoDb` (created after first run of the application)

## License

MIT 