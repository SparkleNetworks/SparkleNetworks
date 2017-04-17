CREATE PROCEDURE [dbo].[GetTimelineListIdTeam]
	@networkId int,
	@dateMax DateTime,
	@teamId int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.TeamId = @teamId
ORDER BY T.CreateDate DESC, T.Id ASC
