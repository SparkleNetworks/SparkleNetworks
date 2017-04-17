ALTER TABLE [dbo].[SocialNetworkConnections]
    ADD CONSTRAINT [FK_SocialNetworkConnections_User] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id])
	ON DELETE NO ACTION ON UPDATE NO ACTION;

