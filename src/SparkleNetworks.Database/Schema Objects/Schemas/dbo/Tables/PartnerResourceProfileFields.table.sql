
-- Resulting entity class must implement IProfileFieldValue

CREATE TABLE [dbo].[PartnerResourceProfileFields]
(
    [Id]                INT             NOT NULL IDENTITY (1, 1),
    [PartnerResourceId] INT             NOT NULL,
    [ProfileFieldId]    INT             NOT NULL,
    [Value]             NVARCHAR(MAX)   NOT NULL,
    [DateCreatedUtc]    SMALLDATETIME   NOT NULL,
    [DateUpdatedUtc]    SMALLDATETIME   NULL,
    [UpdateCount]       SMALLINT        NOT NULL,
    [Source]            TINYINT         NOT NULL DEFAULT 1,   -- latest source for the value (user input, linkedin, ...)
    [Data]              NVARCHAR(MAX)   NULL,

    CONSTRAINT [PK_dbo_PartnerResourcesProfileFields] PRIMARY KEY CLUSTERED ([Id]),

    CONSTRAINT [FK_dbo_PartnerResourcesProfileFields_PartnerResources] FOREIGN KEY ([PartnerResourceId]) REFERENCES [dbo].[PartnerResources] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_dbo_PartnerResourcesProfileFields_ProfileFields] FOREIGN KEY ([ProfileFieldId]) REFERENCES [dbo].[ProfileFields] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
)
GO
