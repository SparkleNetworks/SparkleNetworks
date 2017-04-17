CREATE TABLE [dbo].[StripeTransactions]
(
    [Id]                    INT             NOT NULL IDENTITY (1, 1),
    [UserId]                INT             NOT NULL,
    [NetworkId]             INT             NOT NULL,

    [TokenId]               NVARCHAR(100)   NOT NULL,
    [ChargeId]              NVARCHAR(100)   NULL,
    [CustomerId]            NVARCHAR(100)   NULL,
    [CardId]                NVARCHAR(100)   NULL,

    [AmountUsd]             NUMERIC (10, 3) NULL,
    [AmountEur]             NUMERIC (10, 3) NULL,
    [DateCreatedUtc]        DATETIME        NOT NULL,
    [DateUpdatedUtc]        DATETIME        NULL,

    [IsChargeCreated]       BIT             NOT NULL,
    [IsChargeCaptured]      BIT             NOT NULL,

    CONSTRAINT [PK_dbo_StripeTransactions_Id] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_dbo_StripeTransactions_Networks] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]),
	CONSTRAINT [FK_dbo_StripeTransactions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
)
