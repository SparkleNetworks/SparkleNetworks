
CREATE PROCEDURE [dbo].[GetTimelineItemIdsByImportedIdExpression]
	@networkId int,
	@importedIdExpression nvarchar(420)
AS

SELECT [Id],[ImportedId]
FROM dbo.TimelineItems
WHERE NetworkId = @networkId AND ImportedId LIKE @importedIdExpression
