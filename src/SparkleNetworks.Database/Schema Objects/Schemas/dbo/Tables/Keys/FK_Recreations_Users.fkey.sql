ALTER TABLE [dbo].[Recreations]
    ADD CONSTRAINT [FK_Recreations_Users] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

