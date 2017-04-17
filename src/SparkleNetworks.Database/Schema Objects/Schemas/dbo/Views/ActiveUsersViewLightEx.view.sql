CREATE VIEW [dbo].[ActiveUsersViewLightEx]
AS
	SELECT
		U.[Id],U.[Login],U.[FirstName],U.[LastName],U.[Gender],
		U.[CompanyID], C.Name CompanyName,C.Alias CompanyAlias,
		U.[JobId],J.Libelle JobName, J.Alias JobAlias,
		U.[AccountClosed],U.[CompanyAccessLevel],U.[NetworkAccessLevel]
	FROM [Users] U
	LEFT JOIN Jobs J ON J.Id = U.JobId
	LEFT JOIN Companies C ON C.ID = U.CompanyID
	where ([AccountClosed] = 0 OR AccountClosed IS NULL) AND [CompanyAccessLevel] > 0 AND [NetworkAccessLevel] > 0
