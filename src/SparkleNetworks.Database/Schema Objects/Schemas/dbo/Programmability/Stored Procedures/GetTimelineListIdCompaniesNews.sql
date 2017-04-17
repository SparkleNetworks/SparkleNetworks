CREATE PROCEDURE [dbo].[GetTimelineListIdCompaniesNews]
	@networkId int,
	@dateMax DateTime
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.ItemType = 0 AND T.PrivateMode <= 0 AND CompanyId > 0
ORDER BY T.CreateDate DESC, T.Id ASC
