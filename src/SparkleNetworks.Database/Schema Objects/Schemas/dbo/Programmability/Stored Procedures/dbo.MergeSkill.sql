
CREATE PROCEDURE dbo.MergeSkill
    @skillToDelete int,
    @skillToKeep   int
AS
BEGIN
    SET NOCOUNT ON;

    update dbo.CompanySkills       set SkillId = @skillToKeep     where SkillId = @skillToDelete
    update dbo.UserSkills          set SkillId = @skillToKeep     where SkillId = @skillToDelete    
    update dbo.GroupSkills         set SkillId = @skillToKeep     where SkillId = @skillToDelete
    update dbo.ResumeSkills        set SkillId = @skillToKeep     where SkillId = @skillToDelete
    update dbo.TimelineItemSkills  set SkillId = @skillToKeep     where SkillId = @skillToDelete
    delete from dbo.Skills         where Id = @skillToDelete
    
END
GO
