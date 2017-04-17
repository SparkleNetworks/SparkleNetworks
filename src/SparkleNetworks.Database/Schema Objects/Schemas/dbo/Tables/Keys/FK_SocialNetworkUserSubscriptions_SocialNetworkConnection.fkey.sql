ALTER TABLE [dbo].[SocialNetworkUserSubscriptions]
    ADD CONSTRAINT [FK_SocialNetworkUserSubscriptions_SocialNetworkConnection] FOREIGN KEY ([SocialNetworkConnectionsId]) REFERENCES [dbo].[SocialNetworkConnections] ([Id])
	ON DELETE NO ACTION ON UPDATE NO ACTION;

