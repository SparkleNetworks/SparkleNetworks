CREATE PROCEDURE [dbo].[FindApplicationById2]
	@ApplicationId int
AS
	SELECT
		A.[Id], A.Status,
		P.Id ProductId, P.Name ProductName,
		U.Id UniverseId, U.Name UniverseName, U.Status UniverseStatus,
		H.Id HostId, H.Name HostName
	FROM [dbo].Applications A
	INNER JOIN [dbo].Products P ON P.Id = A.ProductId
	INNER JOIN [dbo].Universes U ON U.Id = A.UniverseId
	INNER JOIN [dbo].Hosts H ON H.Id = A.HostId
	WHERE A.Id = @ApplicationId

RETURN 0