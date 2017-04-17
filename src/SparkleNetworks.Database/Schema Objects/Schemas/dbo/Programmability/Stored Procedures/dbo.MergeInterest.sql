
CREATE PROCEDURE dbo.MergeInterest
    @idToDelete int,
    @idToKeep   int
AS
BEGIN
    SET NOCOUNT ON;

    update dbo.UserInterests       set InterestId = @idToKeep     where InterestId = @idToDelete    
    update dbo.GroupInterests      set InterestId = @idToKeep     where InterestId = @idToDelete
    delete from dbo.Interests      where Id = @idToDelete
    
END
GO
