CREATE OR ALTER PROCEDURE sp_UpdateTodo
    @Id INT,
    @Title NVARCHAR(200) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @IsCompleted BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Todo
    SET 
        Title = ISNULL(@Title, Title),
        Description = ISNULL(@Description, Description),
        IsCompleted = ISNULL(@IsCompleted, IsCompleted),
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO 