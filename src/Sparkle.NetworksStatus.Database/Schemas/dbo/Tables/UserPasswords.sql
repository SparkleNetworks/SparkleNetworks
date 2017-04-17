
CREATE TABLE [dbo].[UserPasswords]
(
    [Id]                        INT NOT NULL IDENTITY(1,1),
    UserId                      INT NOT NULL,
    DateCreatedUtc              DATETIME         NOT NULL,
    PasswordValue               varchar(400)     NOT NULL,
    PasswordType                varchar(40)      NOT NULL,
    DateLockedUtc               DATETIME         NULL,
    VerifiedUsages              INT              NOT NULL DEFAULT 0,
    LastVerifiedUsageDateUtc    DATETIME         NULL,
    FirstFailedTentativeDateUtc DATETIME         NULL,
    LastFailedTentativeDateUtc  DATETIME         NULL,
    FailedTentatives            INT              NOT NULL DEFAULT 0,

    CONSTRAINT PK_dbo_UserPasswords PRIMARY KEY (Id),
    CONSTRAINT FK_dbo_UserPasswords_User FOREIGN KEY (UserId) REFERENCES dbo.Users (Id),
)
GO
