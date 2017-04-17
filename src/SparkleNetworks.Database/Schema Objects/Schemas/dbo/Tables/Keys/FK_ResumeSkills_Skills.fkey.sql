ALTER TABLE [dbo].[ResumeSkills]
    ADD CONSTRAINT [FK_ResumeSkills_Skills] FOREIGN KEY ([SkillId]) REFERENCES [dbo].[Skills] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

