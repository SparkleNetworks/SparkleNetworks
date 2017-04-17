CREATE TABLE [dbo].[ConfigKeys]
(
	Id int NOT NULL PRIMARY KEY IDENTITY(1,1),
	Name nvarchar(64) NOT NULL UNIQUE,
	BlitableType varchar(32) NOT NULL,
	Summary nvarchar(512) NOT NULL,
	IsRequired bit NOT NULL,
	IsCollection bit NOT NULL,
	DefaultValue nvarchar(4000) NULL
)
