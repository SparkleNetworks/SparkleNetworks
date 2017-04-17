ALTER TABLE [dbo].[Logs]
    ADD CONSTRAINT [FK_Logs_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

