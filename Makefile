.PHONY: start-db stop-db build run clean help watch test test-unit

# Variables
DOCKER_COMPOSE = docker-compose
DOTNET = dotnet

# Default target
help:
	@echo "Available commands:"
	@echo "  make start-db          - Start SQL Server container"
	@echo "  make stop-db           - Stop and remove containers"
	@echo "  make build             - Build the application"
	@echo "  make run               - Run the application (creates and seeds DB automatically)"
	@echo "  make watch             - Run the application with hot reload"
	@echo "  make test              - Run all tests"
	@echo "  make test-unit         - Run unit tests only"
	@echo "  make clean             - Clean build artifacts"
	@echo "  make all               - Start DB, build and run the application"
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
	@echo "Building application..."
	$(DOTNET) build

# Run the application
run:
	@echo "Running application..."
	cd MyProject.Api && $(DOTNET) run

# Run with hot reload
watch:
	@echo "Running application with hot reload..."
	cd MyProject.Api && $(DOTNET) watch run

# Run all tests
test:
	@echo "Running all tests..."
	$(DOTNET) test

# Run unit tests only
test-unit:
	@echo "Running unit tests only..."
	$(DOTNET) test MyProject.Api.UnitTests/MyProject.Api.UnitTests.csproj --verbosity normal

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	$(DOTNET) clean
	rm -rf MyProject.Api/bin MyProject.Api/obj

# Run everything
all: start-db build run 