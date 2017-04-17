
CREATE PROCEDURE [dbo].[GetUniversesDomainNames]
	@UniverseId int
AS

SELECT Id, UniverseId, DomainName, RedirectToMain
FROM dbo.UniverseDomainNames
WHERE UniverseId = @UniverseId
