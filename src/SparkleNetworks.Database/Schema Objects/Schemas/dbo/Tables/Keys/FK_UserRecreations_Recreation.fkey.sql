ALTER TABLE [dbo].[UserRecreations]
    ADD CONSTRAINT [FK_UserRecreations_Recreation] FOREIGN KEY ([RecreationId]) REFERENCES [dbo].[Recreations] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

