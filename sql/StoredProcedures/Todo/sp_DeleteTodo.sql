CREATE OR ALTER PROCEDURE sp_DeleteTodo
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM Todo
    WHERE Id = @Id;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO 