CREATE TABLE [dbo].[LostItems] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Title]   NVARCHAR (100) NOT NULL,
    [Message] NVARCHAR(4000) NOT NULL,
    [Date]    DATETIME       NOT NULL,
    [State]   BIT            NOT NULL,
    [UserId]  INT            NOT NULL,
	[NetworkId]   int            not null
);
