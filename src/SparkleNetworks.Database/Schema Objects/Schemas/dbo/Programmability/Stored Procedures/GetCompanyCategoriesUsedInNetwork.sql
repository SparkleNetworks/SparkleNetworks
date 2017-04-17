
CREATE PROCEDURE [dbo].[GetCompanyCategoriesUsedInNetwork]
	@networkId int = 0
AS

select cc.Id, cc.Name
from dbo.CompanyCategories cc
where Id in (
	select distinct CategoryId
	from dbo.Companies
	where networkid = @networkId
)
order by cc.Name
