CREATE VIEW [dbo].[ApplicationsView] AS 
	SELECT A.[Id], P.Id ProductId, P.Name ProductName, U.Id UniverseId, U.Name UniverseName, H.Id HostId, H.Name HostName
	FROM [dbo].Applications A
	INNER JOIN [dbo].Products P ON P.Id = A.ProductId
	INNER JOIN [dbo].Universes U ON U.Id = A.UniverseId
	INNER JOIN [dbo].Hosts H ON H.Id = A.HostId