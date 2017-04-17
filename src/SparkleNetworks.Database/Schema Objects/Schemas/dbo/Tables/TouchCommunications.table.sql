CREATE TABLE [dbo].[TouchCommunications] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Type]            INT            NOT NULL,
    [Date]            DATETIME       NOT NULL,
    [BackgroundColor] NCHAR (10)     NOT NULL,
    [BackgroundUrl]   NVARCHAR (MAX) NOT NULL,
	[NetworkId]   int            not null
);
