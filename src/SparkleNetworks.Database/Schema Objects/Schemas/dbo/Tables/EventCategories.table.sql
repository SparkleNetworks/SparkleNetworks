
CREATE TABLE [dbo].[EventCategories]
(
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [ParentId] INT           NOT NULL,
    [Name]     NVARCHAR (50) NOT NULL,
    [NetworkId]   int            null,
    Alias      nvarchar(50)  null,

    CONSTRAINT PK_eura_EventsCategories PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT FK_EventCategories_NetworkId FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    --CONSTRAINT UC_dbo_EventCategories_UniqueName UNIQUE (NetworkId, Name),
    --CONSTRAINT UC_dbo_EventCategories_UniqueAlias UNIQUE (NetworkId, Alias),
);
