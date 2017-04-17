
CREATE PROCEDURE dbo.FindDuplicateSkills
AS
BEGIN

	select s.*,
		(select COUNT(UserId)         from UserSkills us where us.SkillId = s.Id) Users,
		(select COUNT(CompanyId)      from CompanySkills cs where cs.SkillId = s.Id) Companies,
		(select COUNT(GroupId)        from GroupSkills gs where gs.SkillId = s.Id) Groups,
		(select COUNT(TimelineItemId) from TimelineItemSkills ts where ts.SkillId = s.Id) Titems,
		(select COUNT(ResumeId)       from ResumeSkills rs where rs.SkillId = s.Id) Resumes
	from Skills s
	inner join
		(
			select tagname, COUNT(*) n from Skills
			group by tagname
		) qry
		on qry.TagName = s.TagName AND qry.n > 1
	order by tagname

END
