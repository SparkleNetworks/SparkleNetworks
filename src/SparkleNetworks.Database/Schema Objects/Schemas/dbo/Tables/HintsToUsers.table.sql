
CREATE TABLE dbo.HintsToUsers
(
    HintId            INT NOT NULL,
    UserId            INT NOT NULL,
    DateDismissedUtc  datetime null,
    
    CONSTRAINT PK_dbo_HintsToUsers PRIMARY KEY (HintId, UserId),
    CONSTRAINT FK_dbo_HintsToUsers_User FOREIGN KEY (UserId) REFERENCES dbo.Users (Id),
    CONSTRAINT FK_dbo_HintsToUsers_Hint FOREIGN KEY (HintId) REFERENCES dbo.Hints (Id),
)
GO

