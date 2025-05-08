.PHONY: start-db stop-db build run clean help watch test

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
	@echo "  make clean             - Clean build artifacts"
	@echo "  make all               - Start DB, build and run the application"

# Start database containers
start-db:
	@echo "Starting database containers..."
	$(DOCKER_COMPOSE) up -d
	@echo "Database is running at localhost:1433"
	@echo "Adminer is available at http://localhost:8080"

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

# Run tests
test:
	@echo "Running tests..."
	$(DOTNET) test

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	$(DOTNET) clean
	rm -rf MyProject.Api/bin MyProject.Api/obj

# Run everything
all: start-db build run 