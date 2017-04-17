
CREATE TABLE [dbo].[CompanyTags]
(
	[Id]                INT         IDENTITY(1, 1) NOT NULL,
	[CompanyId]         INT         NOT NULL,
	[TagId]             INT         NOT NULL,
	[DateCreatedUtc]    DATETIME    NOT NULL,
	[CreatedByUserId]   INT         NOT NULL,
	[DateDeletedUtc]    DATETIME    NULL,
	[DeletedByUserId]   INT         NULL,
	[DeleteReason]      TINYINT     NULL,
    SortOrder           int         not null default 0,

	CONSTRAINT [PK_dbo_CompanyTags] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_dbo_CompanyTags_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([Id]),
	CONSTRAINT [FK_dbo_CompanyTags_Tags] FOREIGN KEY ([TagId]) REFERENCES [dbo].[TagDefinitions] ([Id]),
	CONSTRAINT [FK_dbo_CompanyTags_CreatedByUser] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT [FK_dbo_CompanyTags_DeletedByUser] FOREIGN KEY ([DeletedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT [UC_dbo_CompanyTags] UNIQUE ([CompanyId], [TagId]),
	CONSTRAINT [CC_dbo_CompanyTags_Deleted] CHECK
	(
		[DateDeletedUtc] IS NULL AND [DeletedByUserId] IS NULL AND [DeleteReason] IS NULL
		OR
		[DateDeletedUtc] IS NOT NULL AND [DeletedByUserId] IS NOT NULL AND [DeleteReason] IS NOT NULL
	),
)
