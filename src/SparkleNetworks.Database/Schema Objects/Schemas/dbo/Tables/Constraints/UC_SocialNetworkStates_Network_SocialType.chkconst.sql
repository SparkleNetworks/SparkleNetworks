ALTER TABLE [dbo].[SocialNetworkStates]
	ADD CONSTRAINT [UC_SocialNetworkStates_Network_SocialType] 
	UNIQUE (NetworkId, SocialNetworkType)
