
CREATE TABLE [dbo].[AdCategories]
(
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [ParentId]    INT           NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    [NetworkId]   int            null,
	Alias         nvarchar(50)  null, -- null because added later

    CONSTRAINT [PK_eura_AdsCategories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AdCategories_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
