CREATE VIEW [dbo].[ActiveUsersViewLight]
AS
	SELECT [Id],[Login],[FirstName],[LastName],[Gender],[CompanyID],[JobId],[AccountClosed],[CompanyAccessLevel],[NetworkAccessLevel]
	FROM [Users]
	where ([AccountClosed] = 0 OR AccountClosed IS NULL) AND [CompanyAccessLevel] > 0 AND [NetworkAccessLevel] > 0
