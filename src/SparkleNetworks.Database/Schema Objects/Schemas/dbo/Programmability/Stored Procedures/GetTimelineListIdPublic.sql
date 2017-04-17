CREATE PROCEDURE [dbo].[GetTimelineListIdPublic]
	@networkId int,
	@dateMax DateTime
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.ItemType != 2
ORDER BY T.CreateDate DESC, T.Id ASC
