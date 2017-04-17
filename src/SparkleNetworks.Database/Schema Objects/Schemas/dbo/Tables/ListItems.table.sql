CREATE TABLE [dbo].[ListItems] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [ListId]          INT            NOT NULL,
    [Text]            NVARCHAR (MAX) NOT NULL,
    [CreatedByUserId] INT            NOT NULL
);

