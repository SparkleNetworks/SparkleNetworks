
CREATE PROCEDURE [dbo].[DeleteJob]
    @deleteJobId int = 0,
    @targetJobId int
AS
BEGIN
    SET NOCOUNT ON;
    
    update dbo.Users set JobId = @targetJobId where JobId = @deleteJobId
    declare @affectedUsers int = @@ROWCOUNT

    delete from dbo.Jobs where Id = @deleteJobId
    declare @deletedJobs int = @@ROWCOUNT

    select @affectedUsers as [AffectedUsers], @deletedJobs as [DeletedJobs]
END
