
CREATE TABLE [dbo].[TagCategories]
(
    [Id]                INT             NOT NULL IDENTITY (1, 1),
    [Name]              NVARCHAR(150)   NOT NULL,
    [Alias]             NVARCHAR(150)   NOT NULL,
    [CanUsersCreate]    BIT             NOT NULL,
	[NetworkId]         INT             NULL,
	[Rules]             NVARCHAR(MAX)   NULL,
    SortOrder           int             not null default 0,

    CONSTRAINT [PK_dbo_TagCategories] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [UC_dbo_TagCategories_Name] UNIQUE ([Name]),
	CONSTRAINT FK_dbo_TagCategories_Network FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
)
