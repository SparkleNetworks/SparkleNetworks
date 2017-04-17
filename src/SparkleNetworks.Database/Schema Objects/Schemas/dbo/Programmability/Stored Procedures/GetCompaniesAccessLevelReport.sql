
CREATE PROCEDURE [dbo].[GetCompaniesAccessLevelReport]
	@networkId int = 0
AS

;WITH 
Level0
AS
(
	SELECT u.CompanyID, COUNT(u.Id) Level0
	FROM dbo.Users u
	WHERE u.CompanyAccessLevel = 0 AND u.NetworkAccessLevel > 0 AND u.AccountClosed <> 1 AND u.IsEmailConfirmed = 1 AND u.NetworkId = @networkId
	GROUP BY u.CompanyID
),
Level1
AS
(
	SELECT u.CompanyID, COUNT(u.Id) Level1
	FROM dbo.Users u
	WHERE u.CompanyAccessLevel = 1 AND u.NetworkAccessLevel > 0 AND u.IsEmailConfirmed = 1 AND u.NetworkId = @networkId
	GROUP BY u.CompanyID
),
Level2
AS
(
	SELECT u.CompanyID, COUNT(u.Id) Level2
	FROM dbo.Users u
	WHERE u.CompanyAccessLevel = 2 AND u.NetworkAccessLevel > 0 AND u.IsEmailConfirmed = 1 AND u.NetworkId = @networkId
	GROUP BY u.CompanyID
),
Level3
AS
(
	SELECT u.CompanyID, COUNT(u.Id) Level3
	FROM dbo.Users u
	WHERE u.CompanyAccessLevel = 3 AND u.NetworkAccessLevel > 0 AND u.IsEmailConfirmed = 1 AND u.NetworkId = @networkId
	GROUP BY u.CompanyID
),
OtherDisabled
AS
(
	SELECT u.CompanyID, COUNT(u.Id) OtherDisabled
	FROM dbo.Users u
	WHERE (u.NetworkAccessLevel < 1 OR u.IsEmailConfirmed = 0) AND u.NetworkId = @networkId
	GROUP BY u.CompanyID
)

SELECT c.Id, c.Name, c.Alias, COALESCE(Level0.Level0, 0) Level0, COALESCE(Level1.Level1, 0) Level1, COALESCE(Level2.Level2, 0) Level2, COALESCE(Level3.Level3, 0) Level3, COALESCE(OtherDisabled.OtherDisabled, 0) OtherDisabled
FROM dbo.Companies c
LEFT JOIN Level0 on c.ID = Level0.CompanyID
LEFT JOIN Level1 on c.ID = Level1.CompanyID
LEFT JOIN Level2 on c.ID = Level2.CompanyID
LEFT JOIN Level3 on c.ID = Level3.CompanyID
LEFT JOIN OtherDisabled on c.ID = OtherDisabled.CompanyID
WHERE c.NetworkId = @networkId

