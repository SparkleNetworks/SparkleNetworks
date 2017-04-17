CREATE PROCEDURE [dbo].[GetCompaniesPublicationToValidate]
	@networkId int
AS

SELECT COUNT(T.Id)
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND T.DeleteReason IS NULL AND T.PrivateMode = -1