CREATE OR ALTER PROCEDURE sp_GetCompletedTodos
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
    WHERE IsCompleted = 1
    ORDER BY UpdatedAt DESC;
END 