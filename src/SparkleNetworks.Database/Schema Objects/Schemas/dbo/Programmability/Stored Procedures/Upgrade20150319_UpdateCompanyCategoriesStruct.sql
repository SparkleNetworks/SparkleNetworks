
CREATE PROCEDURE [dbo].[Upgrade20150319_UpdateCompanyCategoriesStruct]
AS

SET NOCOUNT ON

IF EXISTS (SELECT TOP 1 1 FROM [dbo].[CompanyCategories] WHERE NetworkId IS NOT NULL)
BEGIN
	print 'Upgrade20150319_UpdateCompanyCategoriesStruct: Post deployement action has already been executed. Not Running.'
	RETURN 0
END

print 'Upgrade20150319_UpdateCompanyCategoriesStruct: beginning upgrade.'
BEGIN TRANSACTION
	DECLARE @CategoryId SMALLINT
	DECLARE @CategoryName NVARCHAR(150)
	DECLARE @RowCount INT

	DECLARE CategoryCursor CURSOR
	FOR (SELECT Id, Name FROM [dbo].[CompanyCategories] WHERE NetworkId IS NULL)

	-- loop through CompanyCategories
	BEGIN TRY
		OPEN CategoryCursor
		SET @RowCount = 0
		FETCH NEXT FROM CategoryCursor INTO @CategoryId, @CategoryName
		print 'Begin CompanyCategories loop'
		WHILE @@FETCH_STATUS = 0
		BEGIN
			print 'Category: ' + @CategoryName
			DECLARE @NetworkId INT

			DECLARE NetworksCursor CURSOR
			FOR (SELECT Id FROM [dbo].[Networks])

			-- loop through all networks
			BEGIN TRY
				OPEN NetworksCursor
				FETCH NEXT FROM NetworksCursor INTO @NetworkId
				print 'Begin Networks loop'
				WHILE @@FETCH_STATUS = 0
				BEGIN
					print 'Network: ' + CAST(@NetworkId AS VARCHAR)
					DECLARE @NewCategoryId SMALLINT

					INSERT INTO [dbo].[CompanyCategories] ([Name], [NetworkId]) VALUES (@CategoryName, @NetworkId)
					SET @NewCategoryId = CAST(SCOPE_IDENTITY() AS SMALLINT)
					UPDATE [dbo].[Companies] SET CategoryId = @NewCategoryId WHERE CategoryId = 0 AND NetworkId = @NetworkId
					UPDATE [dbo].[Companies] SET CategoryId = @NewCategoryId WHERE CategoryId = @CategoryId AND NetworkId = @NetworkId
					UPDATE [dbo].[CompanyRequests] SET CategoryId = @NewCategoryId WHERE CategoryId = 0 AND NetworkId = @NetworkId
					UPDATE [dbo].[CompanyRequests] SET CategoryId = @NewCategoryId WHERE CategoryId = @CategoryId AND NetworkId = @NetworkId
					SET @RowCount = @RowCount + 1;

					FETCH NEXT FROM NetworksCursor INTO @NetworkId
				END
			END TRY

			BEGIN CATCH
				EXECUTE [dbo].[GetErrorInfo]
				CLOSE CategoryCursor
				CLOSE NetworksCursor
				DEALLOCATE CategoryCursor
				DEALLOCATE NetworksCursor
				ROLLBACK TRANSACTION
				RETURN 1
			END CATCH

			CLOSE NetworksCursor
			DEALLOCATE NetworksCursor
			FETCH NEXT FROM CategoryCursor INTO @CategoryId, @CategoryName
		END

		DELETE FROM [dbo].[CompanyCategories] WHERE NetworkId IS NULL
	END TRY

	BEGIN CATCH
		EXECUTE [dbo].[GetErrorInfo]
		CLOSE CategoryCursor
		DEALLOCATE CategoryCursor
		ROLLBACK TRANSACTION
		RETURN 1
	END CATCH

	CLOSE CategoryCursor
	DEALLOCATE CategoryCursor
	print 'Upgrade20150319_UpdateCompanyCategoriesStruct: ' + CAST(@RowCount AS VARCHAR) + ' company categories updated successfuly.'
COMMIT TRANSACTION
RETURN 0
