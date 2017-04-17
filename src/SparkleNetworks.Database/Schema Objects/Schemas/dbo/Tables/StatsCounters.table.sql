
CREATE TABLE [dbo].[StatsCounters]
(
	Id int NOT NULL IDENTITY (1,1) PRIMARY KEY, 
	Category nvarchar(40) not null,
	Name nvarchar(80) not null,

	CONSTRAINT UC_StatsCounters_CategoryAndName UNIQUE NONCLUSTERED (Category, Name)
)
GO

