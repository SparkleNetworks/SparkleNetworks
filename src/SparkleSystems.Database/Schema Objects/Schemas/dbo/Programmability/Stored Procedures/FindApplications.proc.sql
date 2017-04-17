CREATE PROCEDURE [dbo].[FindApplications]
	@Product nvarchar(48),
	@Host nvarchar(48)
AS
	
	SELECT A.[Id], P.Id ProductId, P.Name ProductName, U.Id UniverseId, U.Name UniverseName, H.Id HostId, H.Name HostName
	FROM [dbo].Applications A
	INNER JOIN [dbo].Products P ON P.Id = A.ProductId
	INNER JOIN [dbo].Universes U ON U.Id = A.UniverseId
	INNER JOIN [dbo].Hosts H ON H.Id = A.HostId
	WHERE P.Name = @Product AND H.Name = @Host

RETURN 0
