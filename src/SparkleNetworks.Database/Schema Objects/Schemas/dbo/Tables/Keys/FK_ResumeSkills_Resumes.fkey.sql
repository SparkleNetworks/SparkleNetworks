ALTER TABLE [dbo].[ResumeSkills]
    ADD CONSTRAINT [FK_ResumeSkills_Resumes] FOREIGN KEY ([ResumeId]) REFERENCES [dbo].[Resumes] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;

