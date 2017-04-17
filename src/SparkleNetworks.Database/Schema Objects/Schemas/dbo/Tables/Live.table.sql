CREATE TABLE [dbo].[Live] (
    [Id]       INT      IDENTITY (1, 1) NOT NULL,
    [UserId]   INT      NOT NULL,
    [Status]   TINYINT  NOT NULL,
    [DateTime] DATETIME NOT NULL,
	[NetworkId]   int            not null
);
