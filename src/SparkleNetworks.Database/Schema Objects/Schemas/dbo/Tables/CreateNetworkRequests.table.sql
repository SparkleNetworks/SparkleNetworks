
CREATE TABLE [dbo].[CreateNetworkRequests]
(
	Id             int NOT NULL identity (1,1), 
	Name           nvarchar(80)   not null,
	[Address]      nvarchar(250)  not null,
	ZipCode        nvarchar(20)   not null,
	City           nvarchar(100)  not null,
	[State]        nvarchar(100)  not null,
	Country        nvarchar(150)  not null,
	ContactName    nvarchar(100)  not null,
	ContactEmail   nvarchar(100)  not null,
	ContactPhone   nvarchar(100)  not null,
	CreatedDateUtc datetime       not null,
	Remark         nvarchar(1000) null,
	NetworkId	   int			  not null,

	CONSTRAINT PK_CreateNetworkRequests PRIMARY KEY (Id),
	CONSTRAINT [FK_CreateNetworkRequests_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
)
