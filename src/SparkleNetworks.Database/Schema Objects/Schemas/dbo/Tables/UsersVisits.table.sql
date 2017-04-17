CREATE TABLE [dbo].[UsersVisits] (
    [Id]        INT     IDENTITY (1, 1) NOT NULL,
    [UserId]    INT     NOT NULL,
    [Date]      DATE    NOT NULL,
    [ProfileId] INT     NOT NULL,
    [ViewCount] TINYINT NOT NULL
);

