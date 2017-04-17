
CREATE TABLE [dbo].[SocialNetworkConnections]
(
    [Id]				int NOT NULL identity (1,1),
    [CreatedByUserId]	int not null,
    [Type]				tinyint not NULL,
    [Username]			nvarchar(120) not null,
    [OAuthToken]		nvarchar(250) not null,
    [OAuthVerifier]		nvarchar(250) not null,
    [IsActive]                  bit not null default 1,
    [OAuthTokenDateUtc]         datetime null,
    [OAuthTokenDurationMinutes] int null
)
