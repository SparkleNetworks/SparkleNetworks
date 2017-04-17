
CREATE TABLE [dbo].[aspnet_Membership] (
    [ApplicationId]                          UNIQUEIDENTIFIER NOT NULL,
    [UserId]                                 UNIQUEIDENTIFIER NOT NULL,
    [Password]                               NVARCHAR (128)   NOT NULL,
    [PasswordFormat]                         INT              DEFAULT ((0)) NOT NULL,
    [PasswordSalt]                           NVARCHAR (128)   NOT NULL,
    [MobilePIN]                              NVARCHAR (16)    NULL,
    [Email]                                  NVARCHAR (256)   NULL,
    [LoweredEmail]                           NVARCHAR (256)   NULL,
    [PasswordQuestion]                       NVARCHAR (256)   NULL,
    [PasswordAnswer]                         NVARCHAR (128)   NULL,
    [IsApproved]                             BIT              NOT NULL,
    [IsLockedOut]                            BIT              NOT NULL,
    [CreateDate]                             DATETIME         NOT NULL,
    [LastLoginDate]                          DATETIME         NOT NULL,
    [LastPasswordChangedDate]                DATETIME         NOT NULL,
    [LastLockoutDate]                        DATETIME         NOT NULL,
    [FailedPasswordAttemptCount]             INT              NOT NULL,
    [FailedPasswordAttemptWindowStart]       DATETIME         NOT NULL,
    [FailedPasswordAnswerAttemptCount]       INT              NOT NULL,
    [FailedPasswordAnswerAttemptWindowStart] DATETIME         NOT NULL,
    [Comment]                                NTEXT            NULL,

    CONSTRAINT PK_aspnet_Membership PRIMARY KEY NONCLUSTERED ([UserId] ASC),
    CONSTRAINT FK_aspnet_Membership_Application FOREIGN KEY ([ApplicationId])
        REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_aspnet_Membership_User FOREIGN KEY ([UserId])
        REFERENCES [dbo].[aspnet_Users] ([UserId])
        ON DELETE NO ACTION ON UPDATE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX IX_dbo_aspnet_Membership_UserId ON [dbo].aspnet_Membership ([UserId] ASC )
GO

EXECUTE sp_tableoption @TableNamePattern = N'[dbo].[aspnet_Membership]', @OptionName = N'text in row', @OptionValue = N'3000';
GO
