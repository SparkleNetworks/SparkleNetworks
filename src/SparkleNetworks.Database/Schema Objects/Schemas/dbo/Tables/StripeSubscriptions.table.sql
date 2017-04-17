CREATE TABLE [dbo].[StripeSubscriptions]
(
    [SubscriptionId]    INT NOT NULL,
    [TransactionId]     INT NOT NULL,

    CONSTRAINT [PK_dbo_StripeSubscriptions] PRIMARY KEY ([SubscriptionId], [TransactionId]),
    CONSTRAINT [FK_dbo_StripeSubscriptions_Subscriptions] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscriptions] ([Id]),
    CONSTRAINT [FK_dbo_StripeSubscriptions_StripeTransactions] FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[StripeTransactions] ([Id]),
)
