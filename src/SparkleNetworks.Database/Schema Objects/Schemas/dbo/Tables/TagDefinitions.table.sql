
CREATE TABLE [dbo].[TagDefinitions]
(
    [Id]                INT             NOT NULL IDENTITY (1, 1),
    [NetworkId]         INT             NOT NULL,
    [CategoryId]        INT             NOT NULL,
    [Name]              NVARCHAR(150)   NOT NULL,
    [Alias]             NVARCHAR(150)   NOT NULL,
    [Description]       NVARCHAR(4000)  NULL,
    [CreatedDateUtc]    DATETIME        NOT NULL,
    [CreatedByUserId]   INT             NOT NULL,
    Data                nvarchar(2000)  null, -- this may contain JSON

    CONSTRAINT [PK_dbo_TagDefinitions] PRIMARY KEY CLUSTERED ([Id]),

    CONSTRAINT [FK_dbo_TagDefinitions_Networks] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_dbo_TagDefinitions_TagCategories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[TagCategories] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_dbo_TagDefinitions_Users] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,

    CONSTRAINT [UC_dbo_TagDefinitions_Name] UNIQUE ([Name], CategoryId, NetworkId),
)
GO

CREATE UNIQUE INDEX IX_dbo_TagDefinitions_Main  ON dbo.TagDefinitions (NetworkId, CategoryId, Name, Alias)
GO
CREATE UNIQUE INDEX UX_dbo_TagDefinitions_Alias ON dbo.TagDefinitions ([Alias], NetworkId) 
GO
