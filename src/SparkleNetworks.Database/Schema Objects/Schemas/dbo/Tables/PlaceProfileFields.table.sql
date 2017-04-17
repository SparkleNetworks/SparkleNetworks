
-- Resulting entity class must implement IProfileFieldValue

CREATE TABLE [dbo].[PlaceProfileFields]
(
    [Id]                 INT                NOT NULL IDENTITY (1, 1),
    [PlaceId]            INT                NOT NULL,
    [ProfileFieldId]     INT                NOT NULL,
    [Value]              NVARCHAR(MAX)      NOT NULL,             -- main/display value
    [DateCreatedUtc]     SMALLDATETIME      NOT NULL,             -- 
    [DateUpdatedUtc]     SMALLDATETIME      NULL,                 -- 
    [UpdateCount]        SMALLINT           NOT NULL DEFAULT 0,	  -- nuber of changes of this field
    [Source]             TINYINT            NOT NULL DEFAULT 1,   -- latest source for the value (user input, linkedin, ...)
    [Data]               NVARCHAR(MAX)      NULL,                 -- extra JSON'ish data: serialized string

	CONSTRAINT [PK_dbo_PlaceProfileFields] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo_PlaceProfileFields_Places] FOREIGN KEY ([PlaceId]) REFERENCES [dbo].[Places] ([Id]),
	CONSTRAINT [FK_dbo_PlaceProfileFields_ProfileFields] FOREIGN KEY ([ProfileFieldId]) REFERENCES [dbo].[ProfileFields] ([Id]),
)
