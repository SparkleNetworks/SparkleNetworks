CREATE TABLE [dbo].[PollChoices] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [PollId] INT            NOT NULL,
    [Choice] NVARCHAR (MAX) NOT NULL
);

