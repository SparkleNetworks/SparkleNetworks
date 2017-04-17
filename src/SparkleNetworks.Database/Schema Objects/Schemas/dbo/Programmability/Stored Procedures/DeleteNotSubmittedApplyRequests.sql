CREATE PROCEDURE [dbo].[DeleteNotSubmittedApplyRequests]
	@networkId int
AS

DELETE FROM [dbo].[ApplyRequests]
WHERE NetworkId = @networkId AND DateSubmitedUtc IS NULL
