ALTER TABLE [dbo].[SocialNetworkCompanySubscriptions]
    ADD CONSTRAINT [FK_SocialNetworkCompanySubscriptions_SocialNetworkConnection] FOREIGN KEY ([SocialNetworkConnectionsId]) REFERENCES [dbo].[SocialNetworkConnections] ([Id])
	ON DELETE NO ACTION ON UPDATE NO ACTION;

