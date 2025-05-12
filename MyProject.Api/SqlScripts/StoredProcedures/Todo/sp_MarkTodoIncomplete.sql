CREATE OR ALTER PROCEDURE sp_MarkTodoIncomplete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if Todo exists
    IF NOT EXISTS (SELECT 1 FROM Todo WHERE Id = @Id)
    BEGIN
        -- Return 0 (failure)
        RETURN 0;
    END
    
    DECLARE @CurrentTime DATETIME = GETUTCDATE();
    
    UPDATE Todo
    SET 
        IsCompleted = 0,
        UpdatedAt = @CurrentTime
    WHERE Id = @Id;
    
    -- Return 1 (success)
    RETURN 1;
END 