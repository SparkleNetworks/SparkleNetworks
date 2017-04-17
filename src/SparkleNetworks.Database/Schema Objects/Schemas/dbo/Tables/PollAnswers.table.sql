CREATE TABLE [dbo].[PollAnswers] (
    [Id]       INT IDENTITY (1, 1) NOT NULL,
    [PollId]   INT NOT NULL,
    [ChoiceId] INT NOT NULL,
    [UserId]   INT NOT NULL
);

