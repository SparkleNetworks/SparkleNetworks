
CREATE TABLE dbo.[UserActionKeys]
(
	[Id]              int NOT NULL IDENTITY (1,1),
	[UserId]          int NOT NULL,
	[Action]          nvarchar(32) NOT NULL,
	[DateCreatedUtc]  datetime NOT NULL,
	[DateExpiresUtc]  datetime NULL,
	[MaxUsages]       int NOT NULL DEFAULT 1,
	[RemainingUsages] int NOT NULL DEFAULT 1,
	[Secret]          nvarchar(250) NOT NULL,
	[Result]          BIT NULL,

	CONSTRAINT PK_dbo_UserActionKeys PRIMARY KEY (Id),
	CONSTRAINT FK_dbo_UserActionKeysUser FOREIGN KEY (UserId) REFERENCES dbo.Users (Id),

)
