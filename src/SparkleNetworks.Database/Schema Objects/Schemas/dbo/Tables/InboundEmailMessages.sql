CREATE TABLE [dbo].[InboundEmailMessages]
(
	[Id]					INT				IDENTITY (1, 1) NOT NULL,
	[NetworkId]				INT				NOT NULL,
	[SenderEmailAccount]	NVARCHAR(100)	NOT NULL,
	[SenderEmailTag]		NVARCHAR(100)	NULL,
	[SenderEmailDomain]		NVARCHAR(100)	NOT NULL,
	[ReceiverEmailAccount]	NVARCHAR(100)	NOT NULL,
	[ReceiverEmailTag]		NVARCHAR(100)	NULL,
	[ReceiverEmailDomain]	NVARCHAR(100)	NOT NULL,
	[Provider]				INT				NOT NULL,
	[DateReceivedUtc]		DATETIME		NOT NULL,
	[SourceEmailFileName]	NVARCHAR(50)	NOT NULL,

	[ProviderSpamScore]		FLOAT			NULL,
	[DkimSigned]			BIT				NULL,
	[DkimValid]				BIT				NULL,
	[SpfTestResult]			NVARCHAR(20)	NULL,
	[SpfTestDetail]			NVARCHAR(400)	NULL,
	[Success]				BIT				NOT NULL,

	CONSTRAINT [PK_dbo_InboundEmailMessages]			PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo_InboundEmailMessages_Network]	FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
);
