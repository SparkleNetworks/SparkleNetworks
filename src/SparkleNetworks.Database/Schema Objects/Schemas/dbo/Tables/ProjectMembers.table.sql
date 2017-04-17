CREATE TABLE [dbo].[ProjectMembers] (
    [Id]            INT IDENTITY (1, 1) NOT NULL,
    [ProjectId]     INT NOT NULL,
    [Notifications] INT NULL,
    [Rights]        INT NULL,
    [UserId]        INT NOT NULL
);

