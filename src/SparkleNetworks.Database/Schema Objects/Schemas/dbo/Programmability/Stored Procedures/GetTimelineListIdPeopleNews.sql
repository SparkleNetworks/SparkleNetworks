CREATE PROCEDURE [dbo].[GetTimelineListIdPeopleNews]
	@networkId int,
	@dateMax DateTime
AS

SELECT TOP 40
	T.Id,
	T.PostedByUserId
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.CompanyId IS NULL
ORDER BY T.CreateDate DESC, T.Id ASC
