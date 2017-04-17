
CREATE TABLE [dbo].[ExchangeMaterials] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]       INT            NOT NULL,
    [Date]            DATETIME       NOT NULL,
    [Title]           NVARCHAR (100) NOT NULL,
    [Description]     NVARCHAR(4000) NOT NULL,
    [Type]            TINYINT        NOT NULL,
    [Status]          TINYINT        NOT NULL,
    [CreatedByUserId] INT            NOT NULL,
    [NetworkId]   int            not null,

    CONSTRAINT [PK_eura_ExchangeMaterials] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExchangeMaterials_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ExchangeMaterials_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ExchangeMaterials_User] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
