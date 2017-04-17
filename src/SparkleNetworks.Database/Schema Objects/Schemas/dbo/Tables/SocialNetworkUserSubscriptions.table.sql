
CREATE TABLE [dbo].[SocialNetworkUserSubscriptions]
(
	[Id]							int NOT NULL identity(1,1), 
	[UserId]						int not null,
	[SocialNetworkConnectionsId]	int not null,
	[AutoPublish]					bit not null,
	[ContentContainsFilter]			nvarchar(200) null
)
