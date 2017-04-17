
CREATE TABLE [dbo].[Invited] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [CompanyId]         INT              NOT NULL,
    [Email]             NVARCHAR (150)   NOT NULL,
    [Code]              UNIQUEIDENTIFIER NOT NULL,
    [Date]              DATETIME         NOT NULL,
    [Unregistred]       BIT              NOT NULL default 0,
    [InvitedByUserId]   INT              NOT NULL,
    [UserId]            INT              NULL,
    CompanyAccessLevel  int              null default 0,
    DeletedDateUtc      smalldatetime    null,
    DeletedByUserId     int              null,

    CONSTRAINT PK_eura_Invited PRIMARY KEY ([Id] ASC),
    CONSTRAINT FK_Invited_Inviter FOREIGN KEY ([InvitedByUserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT FK_Invited_User FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT FK_Invited_Company FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]),
    CONSTRAINT FK_Invited_DeletedByUser FOREIGN KEY (DeletedByUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT CC_Invited_Integrity CHECK
    (
        (UserId IS NOT NULL AND DeletedDateUtc IS NULL AND DeletedByUserId IS NULL)
        OR
        (UserId IS NULL AND DeletedDateUtc IS NULL AND DeletedByUserId IS NULL)
        OR
        (UserId IS NULL AND DeletedDateUtc IS NOT NULL AND DeletedByUserId IS NOT NULL)
    ),
)
