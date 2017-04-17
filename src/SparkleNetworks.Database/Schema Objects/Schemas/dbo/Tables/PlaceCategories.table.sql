CREATE TABLE [dbo].[PlaceCategories] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [ParentId]          INT             NOT NULL,
    [Name]              NVARCHAR (50)   NOT NULL,
	Color               nvarchar(7)     null,
	Symbol              nvarchar(50)    null,
	[FoursquareId]      NVARCHAR(30)    NULL, -- null is temporary for migration of old data
	[LastUpdateDateUtc] SMALLDATETIME   NULL,
);

