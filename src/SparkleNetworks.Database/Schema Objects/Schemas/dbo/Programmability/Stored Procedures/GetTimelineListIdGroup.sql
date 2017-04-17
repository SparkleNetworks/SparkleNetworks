CREATE PROCEDURE [dbo].[GetTimelineListIdGroup]
	@networkId int,
	@dateMax DateTime,
	@groupId int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.GroupId = @groupId
ORDER BY T.CreateDate DESC, T.Id ASC
