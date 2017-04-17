
CREATE TABLE [dbo].[TextValue]
(
    [Id]                INT             NOT NULL IDENTITY (1, 1),
    [TextId]            INT             NOT NULL,
    [CultureName]       NVARCHAR(5)     NOT NULL,
    [Title]             NVARCHAR(140)   NOT NULL,
    [Value]             NVARCHAR(MAX)   NOT NULL,
    [DateUpdatedUtc]    DATETIME        NOT NULL,
    [UpdatedByUserId]   INT             NOT NULL,

    CONSTRAINT [PK_dbo_TextValue] PRIMARY KEY CLUSTERED ([Id]),

    CONSTRAINT [FK_dbo_TextValue_Text] FOREIGN KEY ([TextId]) REFERENCES [dbo].[Text] ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [KF_dbo_TextValue_Users] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [dbo].[Users] ON DELETE NO ACTION ON UPDATE NO ACTION,

    CONSTRAINT [UC_dbo_TextValue_TextCulture] UNIQUE ([TextId], [CultureName])
)
