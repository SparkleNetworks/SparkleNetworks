
CREATE TABLE [dbo].[PartnerResourceTags]
(
    [Id]                INT         NOT NULL IDENTITY (1, 1),
    [PartnerResourceId] INT         NOT NULL,
    [TagId]             INT         NOT NULL,
    [DateCreatedUtc]    DATETIME    NOT NULL,
	--[CreatedByUserId]   INT         NOT NULL,
	--[DateDeletedUtc]    DATETIME    NULL,
	--[DeletedByUserId]   INT         NULL,
	--[DeleteReason]      TINYINT     NULL,

    CONSTRAINT [PK_dbo_PartnerResourceTag] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_dbo_PartnerResourceTags_PartnerResources] FOREIGN KEY ([PartnerResourceId]) REFERENCES [dbo].[PartnerResources] ([Id]),
    CONSTRAINT [FK_dbo_PartnerResourceTags_TagDefinitions] FOREIGN KEY ([TagId]) REFERENCES [dbo].[TagDefinitions] ([Id]),
	--CONSTRAINT [FK_dbo_UserTags_CreatedByUser] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	--CONSTRAINT [FK_dbo_UserTags_DeletedByUser] FOREIGN KEY ([DeletedByUserId]) REFERENCES [dbo].[Users] ([Id]),
	--CONSTRAINT [UC_dbo_UserTags] UNIQUE ([UserId], [TagId]),
	--CONSTRAINT [CC_dbo_UserTags_Deleted] CHECK
	--(
	--	[DateDeletedUtc] IS NULL AND [DeletedByUserId] IS NULL AND [DeleteReason] IS NULL
	--	OR
	--	[DateDeletedUtc] IS NOT NULL AND [DeletedByUserId] IS NOT NULL AND [DeleteReason] IS NOT NULL
	--),
)
GO

CREATE UNIQUE INDEX UX_dbo_PartnerResourceTags_Ids ON [dbo].[PartnerResourceTags] ([PartnerResourceId], [TagId])
GO

