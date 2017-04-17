CREATE PROCEDURE [dbo].[Upgrade20141020_LinkedInUserFields]
AS

SET NOCOUNT ON

IF (SELECT TOP 1 1 FROM dbo.UserProfileFields) IS NOT NULL
BEGIN
	print 'Upgrade20141020_LinkedInUserFields: dbo.UserProfileFields is not empty, executing this procedure may cause data redundance.'
	RETURN 0
END

BEGIN TRANSACTION
	DECLARE @Id int
	DECLARE @Site nvarchar(120)
	DECLARE @Phone nvarchar(100)
	DECLARE @About nvarchar(4000)
	DECLARE @City nvarchar(50)
	DECLARE @ZipCode nvarchar(5)
	DECLARE @FavoriteQuotes nvarchar(4000)
	DECLARE @CurrentTarget nvarchar(4000)
	DECLARE @Contribution nvarchar(4000)
	DECLARE @DateUserCreatedUtc SMALLDATETIME
	DECLARE @UserGuid UNIQUEIDENTIFIER
	DECLARE @RowCount INT

	DECLARE UserCursor CURSOR
	FOR (SELECT	Id, Site, Phone, About, City, ZipCode, FavoriteQuotes, CurrentTarget, Contribution, CreatedDateUtc, UserId FROM dbo.Users)

	BEGIN TRY
		OPEN UserCursor
		SET @RowCount = 0
		FETCH NEXT FROM UserCursor INTO @Id, @Site, @Phone, @About, @City, @ZipCode, @FavoriteQuotes, @CurrentTarget, @Contribution, @DateUserCreatedUtc, @UserGuid
		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE @DateCreatedUtc SMALLDATETIME
			IF (@DateUserCreatedUtc IS NOT NULL) BEGIN
				SET @DateCreatedUtc = @DateUserCreatedUtc END
			ELSE BEGIN
				SET @DateCreatedUtc = (SELECT CreateDate FROM dbo.aspnet_Membership WHERE UserId = @UserGuid) END

			IF @Site IS NOT NULL BEGIN
			INSERT INTO dbo.UserProfileFields (UserId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 1, @Site, @DateCreatedUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @Phone IS NOT NULL BEGIN
			INSERT INTO dbo.UserProfileFields (UserId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 2, @Phone, @DateCreatedUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @About IS NOT NULL BEGIN
			INSERT INTO dbo.UserProfileFields (UserId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 3, @About, @DateCreatedUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @City IS NOT NULL BEGIN
			INSERT INTO dbo.UserProfileFields (UserId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 4, @City, @DateCreatedUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @ZipCode IS NOT NULL BEGIN
			INSERT INTO dbo.UserProfileFields (UserId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 5, @ZipCode, @DateCreatedUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @FavoriteQuotes IS NOT NULL BEGIN
			INSERT INTO dbo.UserProfileFields (UserId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 6, @FavoriteQuotes, @DateCreatedUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @CurrentTarget IS NOT NULL BEGIN
			INSERT INTO dbo.UserProfileFields (UserId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 7, @CurrentTarget, @DateCreatedUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @Contribution IS NOT NULL BEGIN
			INSERT INTO dbo.UserProfileFields (UserId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 8, @Contribution, @DateCreatedUtc, 1)
			SET @RowCount = @RowCount + 1 END

			FETCH NEXT FROM UserCursor INTO @Id, @Site, @Phone, @About, @City, @ZipCode, @FavoriteQuotes, @CurrentTarget, @Contribution, @DateUserCreatedUtc, @UserGuid
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
	print 'Upgrade20141020_LinkedInUserFields: ' + CAST(@RowCount AS VARCHAR) + ' rows inserted succesfuly in dbo.UserProfileFields.'
COMMIT TRANSACTION
RETURN 0
