
CREATE TABLE [dbo].[Notifications] (
    [ContactRequest]               BIT             NOT NULL,
    [Publication]                  BIT             NOT NULL,
    [Comment]                      BIT             NOT NULL,
    [EventInvitation]              BIT             NOT NULL,
    [PrivateMessage]               BIT             NOT NULL,
    [StartPage]                    INT             NULL,
    [Newsletter]                   BIT             NULL, -- weekly newsletter
    [DailyNewsletter]              BIT             NULL, -- daily  newsletter
    [Lang]                         BIT             NULL,
    [NotifyOnPersonalEmailAddress] BIT             NULL,
    [Culture]                      NVARCHAR (5)    NULL,
    [UserId]                       INT             NOT NULL,
    [PrivateGroupJoinRequest]      BIT             NOT NULL DEFAULT 1,

    [MailChimp]                    BIT             NOT NULL DEFAULT 1,
    [MailChimpStatus]              NVARCHAR(20)    NULL,
    [MailChimpStatusDateUtc]       SMALLDATETIME   NULL,
    [MainTimelineItems]            BIT             NULL,
    [MainTimelineComments]         BIT             NULL,
    [CompanyTimelineItems]         BIT             NULL,
    [CompanyTimelineComments]      BIT             NULL,

    CONSTRAINT [PK_eura_Notifications] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_Notifications_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
