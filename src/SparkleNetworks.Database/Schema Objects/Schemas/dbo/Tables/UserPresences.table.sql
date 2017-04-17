
CREATE TABLE dbo.UserPresences
(
    [Id]            INT  NOT NULL IDENTITY (1, 1),
    UserId          int  not null,
    [Day]           date not null,
    TimeFrom        datetime not null,
    TimeTo          datetime not null,

    CONSTRAINT PK_dbo_UserPresences PRIMARY KEY (Id),
    CONSTRAINT FK_dbo_UserPresences_User FOREIGN KEY (UserId) REFERENCES dbo.Users (Id),
)
GO

CREATE UNIQUE INDEX UX_dbo_UserPresences_DaysAndUserId 
ON dbo.UserPresences (UserId, [Day])
GO
