CREATE OR ALTER PROCEDURE sp_CreateTodo
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Todo (Title, Description)
    VALUES (@Title, @Description);
    
    SELECT SCOPE_IDENTITY() AS Id;
END
GO 