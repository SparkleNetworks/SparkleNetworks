
CREATE TABLE [dbo].[Buildings] (
    [Id]     INT           IDENTITY (1, 1) NOT NULL,
    [Name]   NVARCHAR (50) NOT NULL,
    [Floors] INT           NOT NULL,
    [NetworkId]   int            not null,

    CONSTRAINT [PK_eura_Buildings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Buildings_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
