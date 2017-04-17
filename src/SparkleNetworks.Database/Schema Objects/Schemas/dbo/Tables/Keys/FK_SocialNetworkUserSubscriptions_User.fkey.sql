ALTER TABLE [dbo].[SocialNetworkUserSubscriptions]
    ADD CONSTRAINT [FK_SocialNetworkUserSubscriptions_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
	ON DELETE NO ACTION ON UPDATE NO ACTION;

