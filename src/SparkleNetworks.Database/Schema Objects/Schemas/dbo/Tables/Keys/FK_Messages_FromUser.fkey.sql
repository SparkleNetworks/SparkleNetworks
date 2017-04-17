ALTER TABLE [dbo].[Messages]
    ADD CONSTRAINT [FK_Messages_FromUser] FOREIGN KEY ([FromUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

