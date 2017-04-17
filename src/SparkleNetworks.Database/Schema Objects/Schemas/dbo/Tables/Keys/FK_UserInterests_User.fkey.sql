ALTER TABLE [dbo].[UserInterests]
    ADD CONSTRAINT [FK_UserInterests_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

