CREATE PROCEDURE [dbo].[FindApplicationById]
	@ApplicationId int
AS
-- 0: Application ID
-- 1: Product ID
-- 2: Product Name
-- 3: Universe ID
-- 4: Universe Name
-- 5: Host ID
-- 6: Host Name
	
	--DECLARE @ApplicationId int
	--SET @ApplicationId = 1

	SELECT A.[Id], P.Id ProductId, P.Name ProductName, U.Id UniverseId, U.Name UniverseName, H.Id HostId, H.Name HostName
	FROM [dbo].Applications A
	INNER JOIN [dbo].Products P ON P.Id = A.ProductId
	INNER JOIN [dbo].Universes U ON U.Id = A.UniverseId
	INNER JOIN [dbo].Hosts H ON H.Id = A.HostId
	WHERE A.Id = @ApplicationId

RETURN 0