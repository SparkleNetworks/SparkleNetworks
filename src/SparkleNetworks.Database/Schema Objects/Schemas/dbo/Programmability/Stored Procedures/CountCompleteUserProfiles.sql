
CREATE PROCEDURE [dbo].[CountCompleteUserProfiles]
	@networkId int
AS

SELECT COUNT(u.Id)
FROM dbo.Users u
INNER JOIN dbo.Companies c ON c.ID = u.CompanyID AND c.NetworkId = @networkId
INNER JOIN dbo.UserProfileFields p on p.UserId = u.Id AND p.ProfileFieldId = 3
WHERE u.NetworkId = @networkId
	AND u.Picture IS NOT NULL
	AND u.CompanyAccessLevel > 0
	AND u.NetworkAccessLevel > 0
	AND u.IsEmailConfirmed > 0
	AND (u.AccountClosed IS NULL OR u.AccountClosed = 0)
	AND c.IsEnabled = 1
	AND DATALENGTH(p.Value) > 400
