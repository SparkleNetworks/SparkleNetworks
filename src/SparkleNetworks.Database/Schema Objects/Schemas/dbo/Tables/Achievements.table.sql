CREATE TABLE [dbo].[Achievements] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ThemeId]     INT            NOT NULL,
    [FamilyId]    INT            NOT NULL,
    [Key]         nvarchar(100)  not null,
    [Title]       NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [Level]       TINYINT        NOT NULL,
    [Target]      INT            NOT NULL,
    [Points]      INT            NOT NULL,
	[NetworkId]   int            null,

	CONSTRAINT [PK_Achievements] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Achievements_NetworkId]
		FOREIGN KEY ([NetworkId])
		REFERENCES [dbo].[Networks] ([Id])
		ON DELETE NO ACTION ON UPDATE NO ACTION,
);
