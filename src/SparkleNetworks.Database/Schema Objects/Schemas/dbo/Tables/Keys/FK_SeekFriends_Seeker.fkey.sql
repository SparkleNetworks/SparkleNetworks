ALTER TABLE [dbo].[SeekFriends]
    ADD CONSTRAINT [FK_SeekFriends_Seeker] FOREIGN KEY ([SeekerId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

