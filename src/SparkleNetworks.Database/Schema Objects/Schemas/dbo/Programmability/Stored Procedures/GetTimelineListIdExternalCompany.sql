CREATE PROCEDURE [dbo].[GetTimelineListIdExternalCompany]
	@networkId int,
	@dateMax DateTime,
	@companyId int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND DeleteDateUtc is null AND T.CreateDate < @dateMax AND T.CompanyId = @companyId AND T.PrivateMode < 0
ORDER BY T.CreateDate DESC, T.Id ASC
