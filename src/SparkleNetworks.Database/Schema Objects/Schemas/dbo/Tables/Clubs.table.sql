
CREATE TABLE [dbo].[Clubs]
(
    [Id]				INT				IDENTITY (1, 1) NOT NULL,
    [Name]				NVARCHAR (100)	NOT NULL,
    [Alias]				NVARCHAR (100)	NOT NULL,
    [Baseline]			NVARCHAR (200)  NULL,
    [About]				NVARCHAR (4000) NULL,
    [Website]			NVARCHAR (120)	NULL,
    [Phone]				NVARCHAR (50)	NULL,
    [Email]				NVARCHAR (100)	NULL,
    [CreatedByUserId]	INT				NOT NULL,
    [CreatedDateUtc]	smalldatetime	not null,
    [NetworkId]			int				null,

    CONSTRAINT PK_dbo_Clubs PRIMARY KEY (Id ASC),
    CONSTRAINT FK_dbo_Clubs_Network FOREIGN KEY ([NetworkId]) REFERENCES dbo.Networks (Id),
    CONSTRAINT FK_dbo_Clubs_CreatedByUser FOREIGN KEY (CreatedByUserId)  REFERENCES dbo.Users (Id),
    CONSTRAINT UC_dbo_Clubs_Alias UNIQUE (Alias),
)
