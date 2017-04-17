
CREATE TABLE dbo.Subscriptions
(
	[Id]               INT NOT NULL IDENTITY (1, 1),
	NetworkId          int not null,
	TemplateId         int not null,
	DateCreatedUtc     datetime not null,
	
	OwnerUserId        int null,
	OwnerCompanyId     int null,

	AppliesToUserId    int null,
	AppliesToCompanyId int null,

	DateBeginUtc       datetime null,
	DateEndUtc         datetime null,
	AutoRenew          bit      not null,
	
	DurationValue            int     not null,
	DurationKind             tinyint not null,
	PriceUsdWithoutVat       numeric (10,3) null,
	PriceUsdWithVat          numeric (10,3) null,
	PriceEurWithoutVat       numeric (10,3) null,
	PriceEurWithVat          numeric (10,3) null,
	Name                     nvarchar(200) not null,
	IsForAllCompanyUsers     bit     not null default 0, -- specifies this subscriptions allows all company users to access

	IsPaid                   bit     not null default 0,
	PaymentMethod            tinyint not null default 0,

	CONSTRAINT PK_dbo_Subscriptions PRIMARY KEY (Id),
	CONSTRAINT FK_dbo_Subscriptions_Network FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
	CONSTRAINT FK_dbo_Subscriptions_Template FOREIGN KEY (TemplateId) REFERENCES dbo.SubscriptionTemplates (Id),

	CONSTRAINT FK_dbo_Subscriptions_OwnerUser        FOREIGN KEY (OwnerUserId)        REFERENCES dbo.Users (Id),
	CONSTRAINT FK_dbo_Subscriptions_OwnerCompany     FOREIGN KEY (OwnerCompanyId)     REFERENCES dbo.Companies (ID),
	CONSTRAINT FK_dbo_Subscriptions_AppliesToUser    FOREIGN KEY (AppliesToUserId)    REFERENCES dbo.Users (Id),
	CONSTRAINT FK_dbo_Subscriptions_AppliesToCompany FOREIGN KEY (AppliesToCompanyId) REFERENCES dbo.Companies (ID),
)
GO
