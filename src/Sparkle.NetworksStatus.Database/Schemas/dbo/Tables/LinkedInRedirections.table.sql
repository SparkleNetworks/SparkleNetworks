CREATE TABLE [dbo].[LinkedInRedirections]
(
    [Id]                UNIQUEIDENTIFIER  NOT NULL,
    [UserId]            INT               NOT NULL,
    [Scope]             INT               NOT NULL,
    [ApiKey]            NVARCHAR(20)      NOT NULL,
    [State]             NVARCHAR(40)      NOT NULL,
    [ReturnUrl]         NVARCHAR(1000)    NOT NULL,
    [DateCreatedUtc]    DATETIME          NOT NULL,

    CONSTRAINT [PK_dbo_Redirections] PRIMARY KEY CLUSTERED ([Id]),
)
GO

CREATE UNIQUE INDEX [IX_dbo_Redirections_State] ON [dbo].[LinkedInRedirections] ([State])
GO