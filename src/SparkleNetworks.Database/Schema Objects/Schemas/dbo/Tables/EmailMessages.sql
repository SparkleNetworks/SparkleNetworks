
CREATE TABLE dbo.EmailMessages
(
	[Id]							INT NOT NULL IDENTITY (1,1),
	NetworkId                       int not null,
	UserId							int NULL,
	DateSentUtc						smalldatetime not null,

	ProviderName					varchar(200) not null,       -- name of the provider used
	ProviderId						varchar(200) null,           -- message id from the provider
	ProviderDeliveryConfirmation	bit null,                    -- SMTP: indicates message was delivered

	SendErrors						smallint not null default 0, -- info from provider: the number of send errors
	SendSucceed						bit not null,                -- info from provider: has the send succeeded?
	LastSendError					nvarchar(800) null,          -- info from provider: last send error

	FirstBounceCode					int null,
	FirstBounceDateUtc				smalldatetime null,
	LastBounceCode					int null,
	LastBounceDateUtc				smalldatetime null,

	Tags							nvarchar(400) null,
	EmailSubject					nvarchar(800) null,
	EmailRecipient					nvarchar(400) null,
	EmailSender						nvarchar(400) null,

	CONSTRAINT PK_dbo_EmailMessages PRIMARY KEY (Id),
	CONSTRAINT FK_dbo_EmailMessages_Network FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
	CONSTRAINT FK_dbo_EmailMessages_User    FOREIGN KEY (UserId)    REFERENCES dbo.Users    (Id),
)
