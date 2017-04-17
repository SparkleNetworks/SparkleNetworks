
CREATE TABLE dbo.UserAuthenticationAttempts
(
	Id                            INT           NOT NULL IDENTITY (1, 1),
	UserId                        int           NOT NULL,
	--EmailAddressAuthenticationId  INT           NOT NULL,
	DateUtc                       datetime      not null,
	ErrorCode                     tinyint       not null,
	RemoteAddressValue            binary(16)    not null,
	RemoteAddressLocation         nvarchar(250) not null,
	UserAgent                     nvarchar(250) null,
	Token                         nvarchar(250) null,
	LastTokenUsage                smalldatetime null,

	CONSTRAINT PK_dbo_UserAuthenticationAttempts PRIMARY KEY (Id),
	CONSTRAINT FK_dbo_UserAuthenticationAttempts_User FOREIGN KEY (UserId) REFERENCES dbo.Users (Id),
)
GO
