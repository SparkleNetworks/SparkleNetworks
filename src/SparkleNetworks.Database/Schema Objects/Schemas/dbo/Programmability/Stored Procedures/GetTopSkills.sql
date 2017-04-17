


CREATE PROCEDURE [dbo].[GetTopSkills]
    @networkId int
AS

;with
TagsByUsers as 
(
    select us.SkillId, coalesce(count(us.UserId), 0) UsersCount
    from dbo.UserSkills us
    left join dbo.Users u on u.Id = us.UserId
    where u.NetworkId = @networkId
    group by us.SkillId
),
TagsByCompanies as 
(
    select cs.SkillId, coalesce(count(cs.CompanyId), 0) CompaniesCount
    from dbo.CompanySkills cs
    left join dbo.Companies c on c.Id = cs.CompanyId 
    where c.NetworkId = @networkId
    group by cs.SkillId
),
Skills as
(
    select s.Id, s.TagName,
        coalesce(us.UsersCount, 0) UsersCount,
        coalesce(cs.CompaniesCount, 0) CompaniesCount
        --coalesce((us.UsersCount + cs.CompaniesCount), 0) TotalCount
    from dbo.Skills s
    left join TagsByUsers us on us.SkillId = s.Id
    left join TagsByCompanies cs on cs.SkillId = s.Id
)

select  Id, TagName, UsersCount, CompaniesCount, (UsersCount + CompaniesCount) TotalCount
from Skills
where UsersCount > 0 OR CompaniesCount > 0
order by TotalCount desc

