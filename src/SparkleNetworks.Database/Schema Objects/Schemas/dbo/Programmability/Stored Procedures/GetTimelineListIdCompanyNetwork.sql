CREATE PROCEDURE [dbo].[GetTimelineListIdCompanyNetwork]
	@networkId int,
	@dateMax DateTime,
	@companyId int
AS

SELECT TOP 40
	T.Id
From TimelineItems T
WHERE T.NetworkId = @networkId AND T.DeleteReason IS NULL AND T.CreateDate < @dateMax AND T.CompanyId = @companyId AND T.PrivateMode = 1
ORDER BY T.CreateDate DESC, T.Id ASC