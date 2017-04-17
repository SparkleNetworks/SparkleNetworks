CREATE TABLE [dbo].[CompanyRelationshipTypes]
(
	[Id]        INT             NOT NULL IDENTITY(1, 1),
	[NetworkId] INT             NOT NULL,
	[Name]      NVARCHAR(150)   NOT NULL,
	[Alias]     NVARCHAR(150)   NOT NULL,
	[Verb]      NVARCHAR(150)   NOT NULL,
	[KnownType] TINYINT         NOT NULL DEFAULT 0,
	[Rules]     NVARCHAR(4000)  NULL,

	CONSTRAINT [PK_CompanyRelationshipTypes] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_CompanyRelationshipTypes_Networks] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]),
	CONSTRAINT [UC_CompanyRelationshipTypes_Alias_NetworkId] UNIQUE ([NetworkId], [Alias]),
)
