CREATE PROCEDURE [dbo].[GetTimelineListIdProfile]
	@networkId int,
	@dateMax DateTime,
	@userId int,
	@isContact int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND ((@isContact = 1 AND T.PrivateMode <= 1) OR (@isContact = 0 AND T.PrivateMode <= 0)) AND (T.UserId = @userId OR (T.PostedByUserId = @userId AND T.UserId IS NULL)) AND T.CompanyId IS NULL AND T.TeamId IS NULL AND T.ProjectId IS NULL
ORDER BY T.CreateDate DESC, T.Id ASC
