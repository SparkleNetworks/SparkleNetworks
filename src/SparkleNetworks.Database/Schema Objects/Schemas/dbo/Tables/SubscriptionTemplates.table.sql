
CREATE TABLE dbo.SubscriptionTemplates
(
	[Id]                     INT NOT NULL IDENTITY (1, 1),
	NetworkId                int not null,

	AllowAutoRenew           bit     not null default 0, -- May be changed. 
	IsDefaultOnAccountCreate bit     not null default 0, -- CANNOT be changed. specifies to create a sub' from this template for all new users
	IsUserSubscribable       bit     not null default 0, -- CANNOT be changed. indicates a user can subscribe from this template

	DurationValue            int     not null,           -- May be changed. 
	DurationKind             tinyint not null,           -- May be changed. 
	PriceUsdWithoutVat       numeric (10,3) null,        -- May be changed. 
	PriceUsdWithVat          numeric (10,3) null,        -- May be changed. 
	PriceEurWithoutVat       numeric (10,3) null,        -- May be changed. 
	PriceEurWithVat          numeric (10,3) null,        -- May be changed. 
	Name                     nvarchar(200) not null,     -- May be changed. 
	IsForAllCompanyUsers     bit     not null default 0, -- CANNOT be changed. specifies this subscriptions allows all company users to access.
	ConfirmEmailTextId       int     null,               -- Customizable and localizable text.
	RenewEmailTextId         int     null,               -- Customizable and localizable text.
	ExpireEmailTextId        int     null,               -- Customizable and localizable text.

    IsSubscriptionEnabled    bit     not null default 1, -- Defines whether a new subscription can be created.

	CONSTRAINT PK_dbo_SubscriptionTemplates PRIMARY KEY (Id),
	CONSTRAINT FK_dbo_SubscriptionTemplates_Network FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
	CONSTRAINT FK_dbo_SubscriptionTemplates_ConfirmText FOREIGN KEY (ConfirmEmailTextId) REFERENCES dbo.[Text] (Id),
	CONSTRAINT FK_dbo_SubscriptionTemplates_RenewText FOREIGN KEY (RenewEmailTextId) REFERENCES dbo.[Text] (Id),
	CONSTRAINT FK_dbo_SubscriptionTemplates_ExpireText FOREIGN KEY (ExpireEmailTextId) REFERENCES dbo.[Text] (Id),
)
GO
