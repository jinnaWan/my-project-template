CREATE OR ALTER PROCEDURE sp_DeleteTodo
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
    
    -- Delete the Todo
    DELETE FROM Todo
    WHERE Id = @Id;
    
    -- Return 1 (success)
    RETURN 1;
END 