
CREATE TABLE [dbo].TimelineItemTags
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

	CONSTRAINT [PK_dbo_TimelineItemTags] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_dbo_TimelineItemTags_Relation] FOREIGN KEY (RelationId) REFERENCES [dbo].TimelineItems ([Id]),
	CONSTRAINT [FK_dbo_TimelineItemTags_Tag] FOREIGN KEY ([TagId]) REFERENCES [dbo].[TagDefinitions] ([Id]),
	CONSTRAINT [FK_dbo_TimelineItemTags_CreatedByUser] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT [FK_dbo_TimelineItemTags_DeletedByUser] FOREIGN KEY ([DeletedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT [UC_dbo_TimelineItemTags] UNIQUE (RelationId, [TagId]),
	CONSTRAINT [CC_dbo_TimelineItemTags_Deleted] CHECK
	(
		[DateDeletedUtc] IS NULL AND [DeletedByUserId] IS NULL AND [DeleteReason] IS NULL
		OR
		[DateDeletedUtc] IS NOT NULL AND [DeletedByUserId] IS NOT NULL AND [DeleteReason] IS NOT NULL
	),
)
