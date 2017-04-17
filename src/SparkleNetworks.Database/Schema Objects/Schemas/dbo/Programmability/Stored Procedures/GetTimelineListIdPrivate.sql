CREATE PROCEDURE [dbo].[GetTimelineListIdPrivate]
	@networkId int,
	@dateMax DateTime,
	@userId int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.PrivateMode <= 1 AND (T.UserId = @userId OR (T.PostedByUserId = @userId AND T.UserId IS NULL)) AND T.TeamId IS NULL AND T.ProjectId IS NULL
ORDER BY T.CreateDate DESC, T.Id ASC
