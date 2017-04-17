CREATE PROCEDURE [dbo].[GetTimelineListIdPlace]
	@networkId int,
	@dateMax DateTime,
	@placeId int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.PlaceId = @placeId
ORDER BY T.CreateDate DESC, T.Id ASC
