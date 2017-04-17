CREATE PROCEDURE [dbo].[GetWallItemById]
	@networkId int,
	@wallId int
AS

SELECT *
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND T.Id = @wallId AND DeleteReason IS NULL
