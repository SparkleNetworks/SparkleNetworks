
CREATE TABLE [dbo].[UserTags]
(
	[Id]                INT         IDENTITY(1, 1) NOT NULL,
	[UserId]            INT         NOT NULL,
	[TagId]             INT         NOT NULL,
	[DateCreatedUtc]    DATETIME    NOT NULL,
	[CreatedByUserId]   INT         NOT NULL,
	[DateDeletedUtc]    DATETIME    NULL,
	[DeletedByUserId]   INT         NULL,
	[DeleteReason]      TINYINT     NULL,
    SortOrder           int         not null default 0,

	CONSTRAINT [PK_dbo_UserTags] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_dbo_UserTags_Companies] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT [FK_dbo_UserTags_Tags] FOREIGN KEY ([TagId]) REFERENCES [dbo].[TagDefinitions] ([Id]),
	CONSTRAINT [FK_dbo_UserTags_CreatedByUser] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT [FK_dbo_UserTags_DeletedByUser] FOREIGN KEY ([DeletedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	CONSTRAINT [UC_dbo_UserTags] UNIQUE ([UserId], [TagId]),
	CONSTRAINT [CC_dbo_UserTags_Deleted] CHECK
	(
		[DateDeletedUtc] IS NULL AND [DeletedByUserId] IS NULL AND [DeleteReason] IS NULL
		OR
		[DateDeletedUtc] IS NOT NULL AND [DeletedByUserId] IS NOT NULL AND [DeleteReason] IS NOT NULL
	),
)
