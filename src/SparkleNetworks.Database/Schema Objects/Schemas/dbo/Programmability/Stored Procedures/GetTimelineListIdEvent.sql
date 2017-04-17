
CREATE PROCEDURE [dbo].[GetTimelineListIdEvent]
	@networkId int,
	@dateMax DateTime,
	@eventId int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.EventId = @eventId
ORDER BY T.CreateDate DESC, T.Id ASC
