
CREATE PROCEDURE [dbo].[GetTimelineCommentIdsByImportedIdExpression]
	@networkId int,
	@importedIdExpression nvarchar(420)
AS

SELECT c.[Id], c.[ImportedId]
FROM dbo.TimelineItemComments c
INNER JOIN dbo.TimelineItems i on i.Id = c.TimelineItemId
WHERE i.NetworkId = @networkId AND c.ImportedId LIKE @importedIdExpression
