
CREATE PROCEDURE [dbo].[UpdateUsersRegisterDateFromMembership]
AS
SET NOCOUNT ON

begin transaction

DECLARE @Id int
DECLARE @UserId uniqueidentifier
DECLARE @MbsCreateDate datetime
DECLARE @UserCreateDate smalldatetime

DECLARE UpdateCursor CURSOR
FOR
	select u.Id, m.UserId, u.CreatedDateUtc, m.CreateDate
	from dbo.Users u
	inner join dbo.aspnet_Membership m on u.UserId = m.UserId
	where u.CreatedDateUtc is null
FOR UPDATE OF
	CreatedDateUtc

begin try
	OPEN UpdateCursor
	FETCH NEXT FROM UpdateCursor INTO @Id, @UserId, @UserCreateDate, @MbsCreateDate
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE dbo.Users
		SET CreatedDateUtc = @MbsCreateDate
		WHERE Id = @Id
	  
		FETCH NEXT FROM UpdateCursor INTO @Id, @UserId, @UserCreateDate, @MbsCreateDate
	END
end try
begin catch
	EXECUTE dbo.GetErrorInfo;
	CLOSE UpdateCursor
	DEALLOCATE UpdateCursor
	rollback transaction
	return 1
end catch

CLOSE UpdateCursor
DEALLOCATE UpdateCursor 

commit transaction
return 0
