
CREATE TABLE [dbo].[SocialNetworkStates]
(
	Id					  int NOT NULL identity(1,1),
	NetworkId			  int not NULL,
	SocialNetworkType	  tinyint not null,
	Username              nvarchar(100) not null,  -- OAuth 1		-- OAuth 2
	OAuthDateUtc          datetime null,		   -- 				-- 
	OAuthRequestToken     nvarchar(250) null,	   -- ????????????? -- 
	OAuthRequestVerifier  nvarchar(250) null,	   -- ????????????? -- 
	OAuthAccessToken      nvarchar(250) null,	   -- ????????????? -- 
	OAuthAccessSecret     nvarchar(250) null,	   -- ????????????? -- 
	LastItemId			  bigint null,
	LastStartDate		  datetime null,
	LastEndDate			  datetime null,
	IsProcessing		  bit not null default 0,
)
