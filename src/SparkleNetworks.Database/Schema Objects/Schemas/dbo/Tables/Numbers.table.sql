CREATE TABLE [dbo].[Numbers] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Name]   NVARCHAR (100) NOT NULL,
    [Number] NVARCHAR (40)  NOT NULL,
	[NetworkId]   int            not null
);
