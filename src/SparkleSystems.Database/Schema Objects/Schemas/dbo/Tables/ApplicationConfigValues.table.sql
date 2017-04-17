CREATE TABLE [dbo].[ApplicationConfigValues]
(
	Id int NOT NULL PRIMARY KEY IDENTITY(1,1),
	ApplicationId int NOT NULL, 
	ConfigKeyId int NOT NULL,
	[Index] int NOT NULL DEFAULT(0),
	Value NVARCHAR(4000) NULL,
)
