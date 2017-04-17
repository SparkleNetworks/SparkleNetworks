
CREATE PROCEDURE dbo.MergeRecreation
    @idToDelete int,
    @idToKeep   int
AS
BEGIN
    SET NOCOUNT ON;

    update dbo.UserRecreations       set RecreationId = @idToKeep     where RecreationId = @idToDelete    
    update dbo.GroupRecreations      set RecreationId = @idToKeep     where RecreationId = @idToDelete
    delete from dbo.Recreations      where Id = @idToDelete
    
END
GO
