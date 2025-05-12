.PHONY: start-db stop-db build run clean help watch test test-unit frontend-dev frontend-build frontend-test frontend-lint

# Variables
DOCKER_COMPOSE = docker-compose
DOTNET = dotnet
NPM = npm

# Default target
help:
	@echo "Available commands:"
	@echo "  make start-db          - Start SQL Server container"
	@echo "  make stop-db           - Stop and remove containers"
	@echo "  make build             - Build the backend application"
	@echo "  make run               - Run the backend application (creates and seeds DB automatically)"
	@echo "  make watch             - Run the backend application with hot reload"
	@echo "  make test              - Run all backend tests"
	@echo "  make test-unit         - Run backend unit tests only"
	@echo "  make clean             - Clean build artifacts"
	@echo "  make all               - Start DB, build and run the application"
	@echo ""
	@echo "Frontend commands:"
	@echo "  make frontend-dev      - Start frontend development server"
	@echo "  make frontend-build    - Build frontend for production"
	@echo "  make frontend-test     - Run frontend tests"
	@echo "  make frontend-lint     - Run frontend linting"
	@echo ""
	@echo "Combined commands:"
	@echo "  make dev               - Run both backend and frontend in development mode"
	@echo ""
	@echo "Database connection info for Azure Data Studio:"
	@echo "  Server: localhost,1433"
	@echo "  Authentication: SQL Login"
	@echo "  Username: sa"
	@echo "  Password: YourStrong@Passw0rd"

# Start database containers
start-db:
	@echo "Starting database containers..."
	$(DOCKER_COMPOSE) up -d
	@echo "Database is running at localhost:1433"
	@echo "Use Azure Data Studio to connect to the database:"
	@echo "  Server: localhost,1433"
	@echo "  Authentication: SQL Login"
	@echo "  Username: sa"
	@echo "  Password: YourStrong@Passw0rd"
	@echo "  Database: TodoDb (after application first run)"

# Stop database containers
stop-db:
	@echo "Stopping database containers..."
	$(DOCKER_COMPOSE) down

# Build the application
build:
	@echo "Building backend application..."
	$(DOTNET) build

# Run the application
run:
	@echo "Running backend application..."
	cd MyProject.Api && $(DOTNET) run

# Run with hot reload
watch:
	@echo "Running backend application with hot reload..."
	cd MyProject.Api && $(DOTNET) watch run

# Run all tests
test:
	@echo "Running all backend tests..."
	$(DOTNET) test

# Run unit tests only
test-unit:
	@echo "Running backend unit tests only..."
	$(DOTNET) test MyProject.Api.UnitTests/MyProject.Api.UnitTests.csproj --verbosity normal

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	$(DOTNET) clean
	rm -rf MyProject.Api/bin MyProject.Api/obj
	@echo "Cleaning frontend build artifacts..."
	rm -rf MyProject.Client/dist MyProject.Client/node_modules/.vite

# Run everything
all: start-db build run 

# Frontend commands
frontend-dev:
	@echo "Starting frontend development server..."
	cd MyProject.Client && $(NPM) run dev

frontend-build:
	@echo "Building frontend for production..."
	cd MyProject.Client && $(NPM) run build

frontend-test:
	@echo "Running frontend tests..."
	cd MyProject.Client && $(NPM) run test

frontend-lint:
	@echo "Running frontend linting..."
	cd MyProject.Client && $(NPM) run lint

# Combined commands
dev:
	@echo "Starting development environment..."
	$(MAKE) start-db
	(cd MyProject.Api && $(DOTNET) watch run) & \
	(cd MyProject.Client && $(NPM) run dev) & \
	wait 