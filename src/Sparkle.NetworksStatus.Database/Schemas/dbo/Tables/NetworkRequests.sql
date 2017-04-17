
CREATE TABLE [dbo].NetworkRequests
(
	[Id]                  INT NOT NULL IDENTITY(1,1),
	DateCreatedUtc        datetime not null,
	RemoteAddress         varchar(39) not null,
	Culture               varchar(5) not null,

	AdminCode             uniqueidentifier default newid(),
	WebId                 uniqueidentifier default newid(),

	ContactFirstname      nvarchar(100) not null,
	ContactLastname       nvarchar(100) not null,
	ContactPhoneNumber    nvarchar(100) not null,
	ContactEmailAccount   nvarchar(100) not null,
	ContactEmailTag       nvarchar(100) null,
	ContactEmailDomain    nvarchar(100) not null,

	NetworkName           nvarchar(100) not null,
	NetworkSize           bigint not null,
	NetworkCity           nvarchar(100) not null,
	NetworkCountry        nvarchar(100) not null,
	NetworkSubdomain      nvarchar(100) not null,

	CONSTRAINT PK_dbo_NetworkRequests PRIMARY KEY (Id),
	CONSTRAINT UC_dbo_NetworkRequests_AdminCode UNIQUE (AdminCode),
	CONSTRAINT UC_dbo_NetworkRequests_WebId UNIQUE (WebId),
)
GO
