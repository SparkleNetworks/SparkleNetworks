CREATE TABLE [dbo].[CompanyRelationships]
(
	[Id]                INT         IDENTITY(1, 1) NOT NULL,
	[TypeId]            INT         NOT NULL,
	[MasterId]          INT         NOT NULL,
	[SlaveId]           INT         NOT NULL,
	[DateCreatedUtc]    DATETIME    NOT NULL,

	CONSTRAINT [PK_CompanyRelationships] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_CompanyRelationships_CompanyRelationshipTypes] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[CompanyRelationshipTypes] ([Id]),
	CONSTRAINT [FK_CompanyRelationships_MasterCompany] FOREIGN KEY ([MasterId]) REFERENCES [dbo].[Companies] ([Id]),
	CONSTRAINT [FK_CompanyRelationships_SlaveCompany] FOREIGN KEY ([SlaveId]) REFERENCES [dbo].[Companies] ([Id])
)
