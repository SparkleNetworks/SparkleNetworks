CREATE TABLE [dbo].[UniverseDomainNames]
(
	Id              int NOT NULL PRIMARY KEY IDENTITY(1,1),
	UniverseId      int NOT NULL,
	DomainName      nvarchar(128) NOT NULL UNIQUE,
	RedirectToMain  bit  not null default 0
)
