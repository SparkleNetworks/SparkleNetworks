CREATE PROCEDURE [dbo].[GetUsersContactIds]
    @userId int

AS
    SELECT DISTINCT COALESCE(nullif(UserId, @userId), ContactId) as ContactId
    FROM dbo.Contacts
    WHERE UserId = @userId or ContactId = @userId
