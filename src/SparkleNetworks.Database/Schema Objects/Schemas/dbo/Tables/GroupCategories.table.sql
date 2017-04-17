
CREATE TABLE [dbo].[GroupCategories] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [ParentId] INT           NOT NULL,
    [Name]     NVARCHAR (50) NOT NULL,

    CONSTRAINT [PK_eura_GroupsCategories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GroupCategories_ParentCategory] FOREIGN KEY ([Id]) REFERENCES [dbo].[GroupCategories] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);

