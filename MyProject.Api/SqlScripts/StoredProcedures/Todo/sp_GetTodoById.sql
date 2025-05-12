CREATE OR ALTER PROCEDURE sp_GetTodoById
    @Id INT
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
    WHERE Id = @Id;
END 