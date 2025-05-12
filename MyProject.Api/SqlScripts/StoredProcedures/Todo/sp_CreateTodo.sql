CREATE OR ALTER PROCEDURE sp_CreateTodo
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL,
    @IsCompleted BIT,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Todo (Title, Description, IsCompleted, CreatedAt, UpdatedAt)
    VALUES (@Title, @Description, @IsCompleted, @CreatedAt, @UpdatedAt);
    
    SELECT SCOPE_IDENTITY() AS Id;
END 