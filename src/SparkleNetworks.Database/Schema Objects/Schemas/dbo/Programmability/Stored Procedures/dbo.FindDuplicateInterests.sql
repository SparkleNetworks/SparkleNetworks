
CREATE PROCEDURE dbo.FindDuplicateInterests
AS
BEGIN

	select s.*,
		(select COUNT(UserId)         from UserInterests us         where us.InterestId = s.Id) Users,
		(select COUNT(GroupId)        from GroupInterests gs        where gs.InterestId = s.Id) Groups
	from dbo.Interests s
	inner join
		(
			select tagname, COUNT(*) n from dbo.Interests
			group by tagname
		) qry
		on qry.TagName = s.TagName AND qry.n > 1
	order by tagname

END
