CREATE PROCEDURE [dbo].[FindApplicationIdByDomainName]
	@Product nvarchar(48),
	@Host nvarchar(48),
	@DomainName nvarchar(128)
AS
	--DECLARE @Product nvarchar(48)
	--DECLARE @Host nvarchar(48)
	--DECLARE @DomainName nvarchar(48)
	--SET @Product = 'Networks'
	--SET @Host = 'Neo'
	--SET @DomainName = 'euratechplus.com'

	SELECT A.[Id]
	FROM [dbo].Applications A
	INNER JOIN [dbo].Products P ON P.Id = A.ProductId
	INNER JOIN [dbo].Universes U ON U.Id = A.UniverseId
	INNER JOIN [dbo].UniverseDomainNames N ON N.UniverseId = A.UniverseId
	INNER JOIN [dbo].Hosts H ON H.Id = A.HostId
	WHERE P.Name = @Product AND N.DomainName = @DomainName AND H.Name = @Host

RETURN 0