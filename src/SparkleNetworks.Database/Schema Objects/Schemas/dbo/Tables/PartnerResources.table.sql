CREATE TABLE [dbo].[PartnerResources]
(
    [Id]                        INT             NOT NULL IDENTITY (1, 1),
    [NetworkId]                 INT             NOT NULL,
    [Name]                      NVARCHAR(140)   NOT NULL,
    [Alias]                     NVARCHAR(140)   NOT NULL,
    [Available]                 BIT             NOT NULL,
    [CreatedByUserId]           INT             NOT NULL,
    [DeletedByUserId]           INT             NULL,
    [DateCreatedUtc]            SMALLDATETIME   NOT NULL,
    [DateDeletedUtc]            SMALLDATETIME   NULL,
    [PictureName]               NVARCHAR(100)   NULL,
    [DateLastPictureUpdateUtc]  DATETIME        NULL,
    [IsApproved]                BIT             NOT NULL,  -- do not inspire from that. please.
    [DateApprovedUtc]           DATETIME        NULL,	   -- do not inspire from that. please.
    [ApprovedByUserId]          INT             NULL,	   -- do not inspire from that. please.

    CONSTRAINT [PK_dbo_PartnerResources] PRIMARY KEY CLUSTERED ([Id]),

    CONSTRAINT [FK_dbo_PartnerResources_Networks] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_dbo_PartnerResources_CreatedByUser] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_dbo_PartnerResources_DeletedByUser] FOREIGN KEY ([DeletedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_dbo_PartnerResources_ApprovedByUser] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,

    CONSTRAINT [UC_dbo_PartnerResources_Alias] UNIQUE ([Alias]),

    CONSTRAINT [CC_dbo_PartnerResourceDeleted] CHECK
    (
        [DeletedByUserId] IS NULL AND [DateDeletedUtc] IS NULL
        OR
        [DeletedByUserId] IS NOT NULL AND [DateDeletedUtc] IS NOT NULL
    ),
    CONSTRAINT [CC_dbo_PartnerResourceApproved] CHECK
    (
        [ApprovedByUserId] IS NULL AND [DateApprovedUtc] IS NULL
        OR
        [ApprovedByUserId] IS NOT NULL AND [DateApprovedUtc] IS NOT NULL
    )
)
