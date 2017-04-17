
CREATE PROCEDURE dbo.FindDuplicateRecreations
AS
BEGIN

	select s.*,
		(select COUNT(UserId)         from UserRecreations us         where us.RecreationId = s.Id) Users,
		(select COUNT(GroupId)        from GroupRecreations gs        where gs.RecreationId = s.Id) Groups
	from dbo.Recreations s
	inner join
		(
			select tagname, COUNT(*) n from dbo.Recreations
			group by tagname
		) qry
		on qry.TagName = s.TagName AND qry.n > 1
	order by tagname

END
