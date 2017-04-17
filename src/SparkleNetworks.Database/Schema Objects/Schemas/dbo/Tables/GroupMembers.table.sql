
CREATE TABLE [dbo].[GroupMembers] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [GroupId]               INT           NOT NULL,
    [Accepted]              SMALLINT      NOT NULL,
    DateJoined              DATE          NULL, -- when Accepted = 4
    [Notifications]         INT           NULL,
    [Rights]                INT           NULL,
    [NotificationFrequency] TINYINT       NULL,
    [UserId]                INT           NOT NULL,
    InvitedByUserId         int           null,
    DateAcceptedUtc         smalldatetime null, -- accepted or rejected in private groups
    AcceptedByUserId        int           null, -- accepted or rejected in private groups
    DateInvitedUtc          smalldatetime null,

    CONSTRAINT [PK_GroupsMembers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GroupMembers_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_GroupsMembers_Group] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_GroupsMembers_InvitedBy] FOREIGN KEY (InvitedByUserId) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_GroupsMembers_AcceptedBy] FOREIGN KEY (AcceptedByUserId) REFERENCES [dbo].[Users] ([Id]),
);

