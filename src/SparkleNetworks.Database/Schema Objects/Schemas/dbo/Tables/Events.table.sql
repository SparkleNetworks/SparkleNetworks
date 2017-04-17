CREATE TABLE [dbo].[Events] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [EventCategory]    INT            NOT NULL,
    [Visibility]       INT            NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [Description]      NVARCHAR(4000) NULL,
    [Picture]          NVARCHAR (50)  NULL,
    [CreateDate]       DATE           NULL,
    [DateEvent]        DATETIME       NULL,
    [DateEndEvent]     DATETIME       NULL,
    [NeedAnswerBefore] DATE           NULL,
    [Website]          NVARCHAR (800) NULL,
    [PlaceId]          INT            NULL,
    [RecurrenceId]     INT            NULL,
    [Room]             NVARCHAR (50)  NULL,
    [CompanyId]        INT            NULL,
    [GroupId]          INT            NULL,
    [TeamId]           INT            NULL,
    [ProjectId]        INT            NULL,
    [Report]           TEXT           NULL,
    [ReportDate]       DATETIME       NULL,
    [CreatedByUserId]  INT            NOT NULL,
	[NetworkId]        int            not null,
    [TicketsWebsite]   NVARCHAR (800) NULL,

	[DeleteReason]      TINYINT        NULL,
    [DeletedByUserId]   INT            NULL,
    [DeleteDateUtc]     SMALLDATETIME  NULL,

	CONSTRAINT [PK_eura_Events] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT C_dbo_Events_DeleteCheck CHECK
    (
        ([DeleteReason] IS NULL AND [DeletedByUserId] IS NULL AND [DeleteDateUtc] IS NULL)
        OR
        ([DeleteReason] IS NOT NULL AND [DeletedByUserId] IS NOT NULL AND [DeleteDateUtc] IS NOT NULL)
    ),
	CONSTRAINT [FK_Events_User] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_Events_Team] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_Events_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_Events_Place] FOREIGN KEY ([PlaceId]) REFERENCES [dbo].[Places] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_Events_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_Events_Group] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_Events_EventCategories] FOREIGN KEY ([EventCategory]) REFERENCES [dbo].[EventCategories] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_Events_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
