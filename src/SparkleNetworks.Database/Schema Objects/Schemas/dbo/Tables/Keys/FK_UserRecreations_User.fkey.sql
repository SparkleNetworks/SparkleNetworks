ALTER TABLE [dbo].[UserRecreations]
    ADD CONSTRAINT [FK_UserRecreations_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

