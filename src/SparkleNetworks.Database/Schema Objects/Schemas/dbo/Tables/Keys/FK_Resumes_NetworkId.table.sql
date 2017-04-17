
ALTER TABLE [dbo].[Resumes]
    ADD CONSTRAINT [FK_Resumes_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
