
CREATE TABLE [dbo].[CareerOpportunities]
(
	[Id]          [int] IDENTITY(1,1) NOT NULL,
	[State]       [int] NOT NULL,
	[CompanyId]   [int] NOT NULL,
	[Type]        [int] NOT NULL,
	[Description] nvarchar(4000) NOT NULL,
	[UserId]      [int] NOT NULL,
	[Ref]         [nvarchar](30) NULL,
	[Date]        [datetime] NOT NULL,
	[Skills]      nvarchar(4000) NULL,
	[Interests]   nvarchar(4000) NULL,
	[Recreations] nvarchar(4000) NULL,
	NetworkId     int not null,

	CONSTRAINT [PK_CareerOpportunities] PRIMARY KEY (Id),
	CONSTRAINT [FK_CareerOpportunities_User] FOREIGN KEY (UserId) REFERENCES Users (Id),
	CONSTRAINT [FK_CareerOpportunities_Company] FOREIGN KEY (CompanyId) REFERENCES Companies (ID),
	CONSTRAINT [FK_CareerOpportunities_Network] FOREIGN KEY (NetworkId) REFERENCES Networks (Id),
)
