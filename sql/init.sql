-- Create the database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TodoDb')
BEGIN
    CREATE DATABASE TodoDb;
END
GO

USE TodoDb;
GO

-- Create Todo table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Todo')
BEGIN
    CREATE TABLE Todo (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX),
        IsCompleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO 