CREATE TABLE [dbo].[UserInterests] (
    [Id]         INT      IDENTITY (1, 1) NOT NULL,
    [InterestId] INT      NOT NULL,
    [Date]       DATETIME NOT NULL,
    [UserId]     INT      NOT NULL
);

