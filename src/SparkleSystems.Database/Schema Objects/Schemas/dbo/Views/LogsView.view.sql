CREATE VIEW [dbo].[LogsView] AS
	SELECT L.UtcDateTime, P.Name Product, H.Name Host, U.Name Universe, L.[Type], L.Path, L.Error, L.RemoteClient, L.Data, L.ApplicationId FROM [dbo].[Logs] L
	INNER JOIN [dbo].Applications A ON A.Id = L.ApplicationId
	INNER JOIN [dbo].Products P ON P.Id = A.ProductId
	INNER JOIN [dbo].Universes U ON U.Id = A.UniverseId
	INNER JOIN [dbo].Hosts H ON H.Id = A.HostId
