﻿
-- Resulting entity class must implement IProfileFieldValue

CREATE TABLE [dbo].CompanyProfileFields
(
    [Id]                 INT                NOT NULL IDENTITY (1, 1),
    CompanyId            INT                NOT NULL,             -- 
    [ProfileFieldId]     INT                NOT NULL,             -- 
    [Value]              NVARCHAR(MAX)      NOT NULL,             -- main/display value
    [DateCreatedUtc]     SMALLDATETIME      NOT NULL,             -- 
    [DateUpdatedUtc]     SMALLDATETIME      NULL,                 -- 
    [UpdateCount]        SMALLINT           NOT NULL DEFAULT 0,	  -- nuber of changes of this field
    [Source]             TINYINT            NOT NULL DEFAULT 1,   -- latest source for the value (user input, linkedin, ...)
    Data                 NVARCHAR(MAX)      NULL,                 -- extra JSON'ish data: serialized string 

    CONSTRAINT [PK_dbo_CompanyProfileFields] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo_CompanyProfileFields_Users] FOREIGN KEY (CompanyId) REFERENCES [dbo].Companies ([Id]),
    CONSTRAINT [FK_dbo_CompanyProfileFields_ProfileFields] FOREIGN KEY ([ProfileFieldId]) REFERENCES [dbo].[ProfileFields] ([Id]),
)
GO
