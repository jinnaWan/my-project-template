CREATE OR ALTER PROCEDURE sp_UpdateTodo
    @Id INT,
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL,
    @IsCompleted BIT,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if Todo exists
    IF NOT EXISTS (SELECT 1 FROM Todo WHERE Id = @Id)
    BEGIN
        -- Return 0 rows affected
        RETURN 0;
    END
    
    -- Update the Todo
    UPDATE Todo
    SET 
        Title = @Title,
        Description = @Description,
        IsCompleted = @IsCompleted,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
    
    -- Return success code (1 = success)
    RETURN 1;
END 