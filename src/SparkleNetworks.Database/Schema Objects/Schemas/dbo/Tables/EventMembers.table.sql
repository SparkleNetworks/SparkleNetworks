
CREATE TABLE [dbo].[EventMembers] (
    [EventId]       INT  NOT NULL,
    [State]         INT  NOT NULL,
    [Notifications] INT  NULL,
    [Rights]        INT  NULL,
    [Comment]       NVARCHAR(4000) NULL,
    [UserId]        INT  NOT NULL,
    DateCreatedUtc  smalldatetime    null,
    DateUpdatedUtc  smalldatetime    null,

    CONSTRAINT [PK_RegisteredToEvent] PRIMARY KEY CLUSTERED ([EventId] ASC, [UserId] ASC),
    CONSTRAINT [FK_EventMembers_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RegisteredToEvent_eura_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION,
);

