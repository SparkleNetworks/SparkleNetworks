CREATE TABLE [dbo].[TeamMembers] (
    [TeamId]        INT      NOT NULL,
    [Date]          DATETIME NOT NULL,
    [Notifications] INT      NULL,
    [Rights]        INT      NULL,
    [UserId]        INT      NOT NULL
);

