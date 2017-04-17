CREATE PROCEDURE [dbo].[FindApplicationId]
	@Product nvarchar(48),
	@Host nvarchar(48),
	@Universe nvarchar(48)
AS

	DECLARE @Id int

	SELECT @id = A.[Id]
	FROM [dbo].Applications A
	INNER JOIN [dbo].Products P ON P.Id = A.ProductId
	INNER JOIN [dbo].Universes U ON U.Id = A.UniverseId
	INNER JOIN [dbo].Hosts H ON H.Id = A.HostId
	WHERE P.Name = @Product AND U.Name = @Universe AND H.Name = @Host

	SELECT @Id
RETURN @Id