
CREATE TABLE [dbo].[TimelineItemComments] (
    [Id]                INT      IDENTITY (1, 1) NOT NULL,
    [TimelineItemId]    INT      NOT NULL,
    [CreateDate]        DATETIME NOT NULL,
    [Text]              NVARCHAR(4000) NOT NULL,
    [PostedByUserId]    INT      NOT NULL,
    [ImportedId]	    NVARCHAR (420) NULL,
    [Extra]             NVARCHAR(4000) NULL,
    [ExtraType]         INT            NULL,
	[InboundEmailId]	INT			   NULL,
);

