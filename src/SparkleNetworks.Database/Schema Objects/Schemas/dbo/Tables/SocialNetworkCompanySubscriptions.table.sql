CREATE TABLE [dbo].[SocialNetworkCompanySubscriptions]
(
	[Id]							int NOT NULL identity(1,1), 
	[CompanyId]						int not null,
	[SocialNetworkConnectionsId]	int not null,
	[AutoPublish]					bit not null,
	[ContentContainsFilter]			nvarchar(200) null
)
