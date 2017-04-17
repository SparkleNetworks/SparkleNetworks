
CREATE TABLE dbo.AdTags
(
	[Id]                INT         IDENTITY(1, 1) NOT NULL,
	[RelationId]        INT         NOT NULL,
	[TagId]             INT         NOT NULL,
	[DateCreatedUtc]    DATETIME    NOT NULL,
	[CreatedByUserId]   INT         NOT NULL,
	[DateDeletedUtc]    DATETIME    NULL,
	[DeletedByUserId]   INT         NULL,
	[DeleteReason]      TINYINT     NULL,
    SortOrder           int         not null default 0,

	CONSTRAINT PK_dbo_AdTags PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT FK_dbo_AdTags_Relation FOREIGN KEY (RelationId) REFERENCES [dbo].Ads ([Id]),
	CONSTRAINT FK_dbo_AdTags_Tag FOREIGN KEY ([TagId]) REFERENCES [dbo].[TagDefinitions] ([Id]),
	CONSTRAINT FK_dbo_AdTags_CreatedByUser FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT FK_dbo_AdTags_DeletedByUser FOREIGN KEY ([DeletedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT UC_dbo_AdTags UNIQUE (RelationId, [TagId]),
	CONSTRAINT CC_dbo_AdTags_Deleted CHECK
	(
		[DateDeletedUtc] IS NULL AND [DeletedByUserId] IS NULL AND [DeleteReason] IS NULL
		OR
		[DateDeletedUtc] IS NOT NULL AND [DeletedByUserId] IS NOT NULL AND [DeleteReason] IS NOT NULL
	),
)
