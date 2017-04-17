CREATE PROCEDURE [dbo].[GetTimelineListIdProject]
	@networkId int,
	@dateMax DateTime,
	@projectId int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.ProjectId = @projectId
ORDER BY T.CreateDate DESC, T.Id ASC
