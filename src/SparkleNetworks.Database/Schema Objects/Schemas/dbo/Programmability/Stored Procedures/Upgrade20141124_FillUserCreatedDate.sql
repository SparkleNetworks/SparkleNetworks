
CREATE PROCEDURE [dbo].Upgrade20141124_FillUserCreatedDate
AS
BEGIN
	SET NOCOUNT ON

	IF (SELECT TOP 1 1 FROM dbo.Users where CreatedDateUtc is null) IS NULL
	BEGIN
		print 'Upgrade20141124_FillUserCreatedDate: all dbo.Users have a CreatedDateUtc.'
		RETURN 0
	END

	BEGIN TRANSACTION
	DECLARE @Id int
	DECLARE @UserId uniqueidentifier
	DECLARE @UserDate smalldatetime
	DECLARE @MbspDate DATETIME
	DECLARE @RowCount INT

	DECLARE UserCursor CURSOR
	FOR 
	(
		select u.Id, u.UserId, u.CreatedDateUtc, m.CreateDate
		from dbo.users u
		left join aspnet_Membership m on m.UserId = u.UserId
		where u.CreatedDateUtc is null and m.CreateDate is not null
	)

	BEGIN TRY
		SET @RowCount = 0
		OPEN UserCursor
		FETCH NEXT FROM UserCursor INTO @Id, @UserId, @UserDate, @MbspDate
		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			update dbo.Users
			set CreatedDateUtc = @MbspDate
			where UserId = @UserId
			
			SET @RowCount = @RowCount + 1

			FETCH NEXT FROM UserCursor INTO @Id, @UserId, @UserDate, @MbspDate
		END
	END TRY

	BEGIN CATCH
		EXECUTE dbo.GetErrorInfo;
		CLOSE UserCursor
		DEALLOCATE UserCursor
		ROLLBACK TRANSACTION
		RETURN 1
	END CATCH

	CLOSE UserCursor
	DEALLOCATE UserCursor
	print 'Upgrade20141124_FillUserCreatedDate: ' + CAST(@RowCount AS VARCHAR) + ' rows updated.'
	COMMIT TRANSACTION
	RETURN 0
END
