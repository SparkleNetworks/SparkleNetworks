
CREATE VIEW [dbo].[UsersView]
AS

select
	u.Id, u.UserId, u.NetworkId, u.Login,
	u.FirstName, u.LastName, u.Gender, u.Picture, u.Email,
	u.AccountClosed, u.CompanyAccessLevel, u.NetworkAccessLevel, u.IsEmailConfirmed,
	u.CompanyID CompanyId, c.Name Company_Name, c.Alias Company_Alias,
	c.IsApproved Company_IsApproved, c.IsEnabled Company_IsEnabled,
	u.JobId JobId, j.Libelle Job_Name, j.Alias JobAlias,
	u.Culture, u.Timezone, u.CreatedDateUtc
from dbo.Users u
inner join dbo.Companies c on c.Id = u.CompanyID
left join dbo.Jobs j on j.Id = u.JobId
