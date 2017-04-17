
-- Resulting entity class must implement IProfileFieldValue

CREATE TABLE [dbo].[UserProfileFields]
(
    [Id]                 INT                NOT NULL IDENTITY (1, 1),
    [UserId]             INT                NOT NULL,             -- 
    [ProfileFieldId]     INT                NOT NULL,             -- 
    [Value]              NVARCHAR(MAX)      NOT NULL,             -- main/display value
    [DateCreatedUtc]     SMALLDATETIME      NOT NULL,             -- 
    [DateUpdatedUtc]     SMALLDATETIME      NULL,                 -- 
    [UpdateCount]        SMALLINT           NOT NULL DEFAULT 0,	  -- number of changes of this field
    [Source]             TINYINT            NOT NULL DEFAULT 1,   -- latest source for the value (user input, linkedin, ...)
    Data                 NVARCHAR(MAX)      NULL,                 -- extra JSON'ish data: serialized string 

    CONSTRAINT [PK_dbo_UserProfileFields] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo_UserProfileFields_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_dbo_UserProfileFields_ProfileFields] FOREIGN KEY ([ProfileFieldId]) REFERENCES [dbo].[ProfileFields] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
