ALTER TABLE [dbo].[ProjectMembers]
    ADD CONSTRAINT [FK_ProjectMembers_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

