
CREATE TABLE [dbo].[Groups] (
    Id                    INT            IDENTITY (1, 1) NOT NULL,
    GroupCategory         INT            NOT NULL,
    IsPrivate             BIT            NOT NULL,
    Name                  NVARCHAR (100) NOT NULL,
    [Description]         NVARCHAR(4000) NOT NULL,
    [Date]                DATETIME       NOT NULL,
    NotificationFrequency TINYINT        NOT NULL,
    CreatedByUserId       INT            NOT NULL,
    NetworkId             INT            not null,

    ImportedId            NVARCHAR (38)  NULL, 
 -- ImportedId is an optional field to store an ancient id when groups are imported.

    IsDeleted             BIT            NOT NULL DEFAULT 0,

    Alias                 nvarchar(100)  null,
 -- Alias is null because is has been added later. There is some code that populates the Alias later.

    CONSTRAINT [PK_eura_Groupes]     PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Groups_NetworkId] FOREIGN KEY ([NetworkId])       REFERENCES [dbo].[Networks] ([Id])        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Groups_Group]     FOREIGN KEY ([GroupCategory])   REFERENCES [dbo].[GroupCategories] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Groups_CreatedBy] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id])           ON DELETE NO ACTION ON UPDATE NO ACTION,
)
GO

CREATE NONCLUSTERED INDEX [IX_Groups_NetworkId] ON [dbo].[Groups] ([Id], NetworkId, IsDeleted, Alias)
GO

-- TODO: uncomment this index later.
--CREATE UNIQUE INDEX UX_dbo_Groups_Alias ON dbo.Groups ([Alias]) 
--GO

ALTER TABLE [dbo].[Groups]
ADD CONSTRAINT [DF_eura_Groups_NotificationFrequency] DEFAULT ((2)) FOR [NotificationFrequency];
GO
