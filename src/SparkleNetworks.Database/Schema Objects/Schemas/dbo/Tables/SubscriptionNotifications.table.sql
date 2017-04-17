
CREATE TABLE [dbo].[SubscriptionNotifications]
(
    [Id]                INT         NOT NULL IDENTITY (1, 1),
    [SubscriptionId]    INT         NOT NULL,
    [DateSendUtc]       DATETIME    NOT NULL,
    [DateSentUtc]       DATETIME    NULL,
	[Status]            TINYINT     NOT NULL,

    CONSTRAINT [PK_dbo_SubscriptionNotifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_dbo_SubscriptionNotifications_Subscriptions] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscriptions],
)
