CREATE TABLE [dbo].[Links] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Link]   NVARCHAR (MAX) NOT NULL,
    [Name]   NVARCHAR (50)  NULL,
    [UserId] INT            NULL
);

