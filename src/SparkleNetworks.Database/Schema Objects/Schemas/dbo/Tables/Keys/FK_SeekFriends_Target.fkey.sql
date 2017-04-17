ALTER TABLE [dbo].[SeekFriends]
    ADD CONSTRAINT [FK_SeekFriends_Target] FOREIGN KEY ([TargetId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

