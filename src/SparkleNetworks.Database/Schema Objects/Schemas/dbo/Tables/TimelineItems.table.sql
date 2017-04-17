
CREATE TABLE [dbo].[TimelineItems] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [ItemType]          INT            NOT NULL,
    [Text]              NVARCHAR (MAX) NOT NULL,
    [CreateDate]        DATETIME       NOT NULL,
    [PrivateMode]       INT            NOT NULL,
    [UserId]            INT            NULL,
    [CompanyId]         INT            NULL,
    [EventId]           INT            NULL,
    [GroupId]           INT            NULL,
    [PlaceId]           INT            NULL,
    [AdId]              INT            NULL,
    [ProjectId]         INT            NULL,
    [TeamId]            INT            NULL,
    [Extra]             NVARCHAR(4000) NULL,
    [ExtraType]         INT            NULL,
    [PostedByUserId]    INT            NOT NULL,
    [NetworkId]         int            not null,
    [ImportedId]        NVARCHAR (420)  NULL,

    [DeleteReason]      TINYINT        NULL,
    [DeletedByUserId]   INT            NULL,
    [DeleteDateUtc]     SMALLDATETIME  NULL,

    [InboundEmailId]    INT            NULL,
	[PartnerResourceId] INT            NULL,

    CONSTRAINT [PK_eura_Wall] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT FK_dbo_TimelineItems_DeletedBy FOREIGN KEY ([DeletedByUserId]) REFERENCES dbo.Users (id),
    CONSTRAINT C_dbo_TimelineItems_DeleteCheck CHECK
    (
        ([DeleteReason] IS NULL AND [DeletedByUserId] IS NULL AND [DeleteDateUtc] IS NULL)
        OR
        ([DeleteReason] IS NOT NULL AND [DeletedByUserId] IS NOT NULL AND [DeleteDateUtc] IS NOT NULL)
    ),
    CONSTRAINT [FK_TimelineItems_UserTimeline] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_TimelineItems_TeamTimeline] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_TimelineItems_ProjectTimeline] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_TimelineItems_PostedBy] FOREIGN KEY ([PostedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_TimelineItems_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_TimelineItems_GroupTimeline] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_TimelineItems_EventTimeline] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_TimelineItems_CompanyTimeline] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_TimelineItems_AdTimeline] FOREIGN KEY ([AdId]) REFERENCES [dbo].[Ads] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_dbo_TimelineItems_InboundEmailId] FOREIGN KEY ([InboundEmailId]) REFERENCES dbo.InboundEmailMessages (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_dbo_TimelineItems_PartnerResources] FOREIGN KEY ([PartnerResourceId]) REFERENCES [dbo].[PartnerResources] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
