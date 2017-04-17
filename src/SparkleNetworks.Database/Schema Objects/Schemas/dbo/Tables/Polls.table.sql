CREATE TABLE [dbo].[Polls] (
    [Id]              INT  IDENTITY (1, 1) NOT NULL,
    [Question]        NVARCHAR(4000) NOT NULL,
    [CreatedByUserId] INT  NOT NULL
);

