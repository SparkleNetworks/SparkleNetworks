ALTER TABLE [dbo].[SocialNetworkStates]
    ADD CONSTRAINT [FK_SocialNetworkStates_Network] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id])
	ON DELETE NO ACTION ON UPDATE NO ACTION;

