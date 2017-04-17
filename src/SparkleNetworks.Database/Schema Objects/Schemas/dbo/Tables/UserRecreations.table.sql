CREATE TABLE [dbo].[UserRecreations] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [RecreationId] INT      NOT NULL,
    [Date]         DATETIME NOT NULL,
    [UserId]       INT      NOT NULL
);

