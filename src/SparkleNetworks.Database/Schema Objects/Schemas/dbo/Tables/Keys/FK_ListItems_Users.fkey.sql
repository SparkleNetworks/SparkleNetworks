ALTER TABLE [dbo].[ListItems]
    ADD CONSTRAINT [FK_ListItems_Users] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

