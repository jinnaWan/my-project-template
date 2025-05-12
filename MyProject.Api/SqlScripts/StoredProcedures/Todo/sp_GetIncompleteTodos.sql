CREATE OR ALTER PROCEDURE sp_GetIncompleteTodos
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
    WHERE IsCompleted = 0
    ORDER BY CreatedAt DESC;
END 