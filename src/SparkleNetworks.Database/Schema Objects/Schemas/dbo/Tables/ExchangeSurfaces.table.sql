
CREATE TABLE [dbo].[ExchangeSurfaces] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]       INT            NOT NULL,
    [Date]            DATETIME       NOT NULL,
    [Status]          TINYINT        NOT NULL,
    [Type]            TINYINT        NOT NULL,
    [Area]            INT            NOT NULL,
    [Title]           NVARCHAR (100) NOT NULL,
    [Description]     NVARCHAR(4000) NOT NULL,
    [CreatedByUserId] INT            NOT NULL,
    [NetworkId]   int            not null,

    CONSTRAINT [PK_eura_ExchangeSurfaces] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExchangeSurfaces_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ExchangeSurfaces_User] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ExchangeSurfaces_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
