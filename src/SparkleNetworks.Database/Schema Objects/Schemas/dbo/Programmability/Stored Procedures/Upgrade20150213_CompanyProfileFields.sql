CREATE PROCEDURE [dbo].[Upgrade20150213_CompanyProfileFields]
AS

SET NOCOUNT ON

IF (SELECT TOP 1 1 FROM dbo.CompanyProfileFields) IS NOT NULL
BEGIN
	print 'Upgrade20150213_CompanyProfileFields: dbo.CompanyProfileFields is not empty, executing this procedure may cause data redundance.'
	RETURN 0
END

BEGIN TRANSACTION
	DECLARE @Id int
	DECLARE @Website nvarchar(100)
	DECLARE @Phone nvarchar(50)
	DECLARE @Email nvarchar(100)
	DECLARE @About nvarchar(max)
	DECLARE @Facebook nvarchar(100)
	DECLARE @Twitter nvarchar(100)
	DECLARE @AngelList nvarchar(100)
	DECLARE @CreatedDateUtc SMALLDATETIME
	DECLARE @RowCount INT

	DECLARE CompanyCursor CURSOR
	FOR (SELECT	Id, Website, Phone, Email, About, Facebook, Twitter, AngelList, CreatedDateUtc FROM dbo.Companies)

	BEGIN TRY
		OPEN CompanyCursor
		SET @RowCount = 0
		FETCH NEXT FROM CompanyCursor INTO @Id, @Website, @Phone, @Email, @About, @Facebook, @Twitter, @AngelList, @CreatedDateUtc
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @Website IS NOT NULL BEGIN
			INSERT INTO dbo.CompanyProfileFields (CompanyId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 1, @Website, @CreatedDateUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @Phone IS NOT NULL BEGIN
			INSERT INTO dbo.CompanyProfileFields (CompanyId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 2, @Phone, @CreatedDateUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @Email IS NOT NULL BEGIN
			INSERT INTO dbo.CompanyProfileFields (CompanyId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 27, @Email, @CreatedDateUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @About IS NOT NULL BEGIN
			INSERT INTO dbo.CompanyProfileFields (CompanyId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 3, @About, @CreatedDateUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @Facebook IS NOT NULL BEGIN
			INSERT INTO dbo.CompanyProfileFields (CompanyId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 28, @Facebook, @CreatedDateUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @Twitter IS NOT NULL BEGIN
			INSERT INTO dbo.CompanyProfileFields (CompanyId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 16, @Twitter, @CreatedDateUtc, 1)
			SET @RowCount = @RowCount + 1 END
			
			IF @AngelList IS NOT NULL BEGIN
			INSERT INTO dbo.CompanyProfileFields (CompanyId, ProfileFieldId, Value, DateCreatedUtc, [Source]) VALUES (@Id, 29, @AngelList, @CreatedDateUtc, 1)
			SET @RowCount = @RowCount + 1 END

			FETCH NEXT FROM CompanyCursor INTO @Id, @Website, @Phone, @Email, @About, @Facebook, @Twitter, @AngelList, @CreatedDateUtc
		END
	END TRY

	BEGIN CATCH
		EXECUTE dbo.GetErrorInfo;
		CLOSE CompanyCursor
		DEALLOCATE CompanyCursor
		ROLLBACK TRANSACTION
		RETURN 1
	END CATCH

	CLOSE CompanyCursor
	DEALLOCATE CompanyCursor
	print 'Upgrade20150213_CompanyProfileFields: ' + CAST(@RowCount AS VARCHAR) + ' rows inserted succesfuly in dbo.CompanyProfileFields.'
COMMIT TRANSACTION
RETURN 0
