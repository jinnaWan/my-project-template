CREATE OR ALTER PROCEDURE sp_GetAllTodos
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Title,
        Description,
        IsCompleted,
        CreatedAt,
        UpdatedAt
    FROM Todo
    ORDER BY CreatedAt DESC;
END
GO 