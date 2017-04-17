
CREATE PROCEDURE [dbo].GetJobsUsedInNetwork
	@networkId int = 0
AS

select j.Id, j.Libelle Name, j.Alias
from dbo.Jobs j
where j.Id in (
	select distinct u.JobId
	from dbo.Users u
	where u.NetworkId = @networkId
)
order by j.Libelle
