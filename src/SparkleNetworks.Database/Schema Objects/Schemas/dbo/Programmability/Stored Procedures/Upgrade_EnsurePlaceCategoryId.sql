
CREATE PROCEDURE [dbo].Upgrade_EnsurePlaceCategoryId
AS

-- Place.CategoryId needs to be not null for structural updates
IF (EXISTS(select top 1 1 from sys.columns where name='Id' and object_id=object_id('dbo.Places')) and EXISTS (SELECT Id FROM dbo.Places WHERE CategoryId IS NULL))
BEGIN
	DECLARE @PlaceCategoryId int
	SELECT TOP 1 @PlaceCategoryId = Id FROM dbo.PlaceCategories
	IF @PlaceCategoryId IS NULL
	BEGIN
		INSERT INTO dbo.PlaceCategories (Name, ParentId)
		VALUES ('Default category', 0)
		SET @PlaceCategoryId = SCOPE_IDENTITY()
	END
	PRINT N'Updating [dbo].[Places] for upgrade...';
	UPDATE dbo.Places SET CategoryId = @PlaceCategoryId
END
