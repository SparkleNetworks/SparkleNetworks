CREATE TABLE [dbo].[TouchCommunicationItems] (
    [Id]          INT      IDENTITY (1, 1) NOT NULL,
    [ParentId]    INT      NOT NULL,
    [Title]       NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(4000) NOT NULL,
    [Start]       DATETIME NOT NULL,
    [End]         DATETIME NOT NULL
);

