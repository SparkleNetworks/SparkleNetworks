
CREATE TABLE [dbo].[StatsCounterHits]
(
	Id int NOT NULL IDENTITY (1,1) primary key, 
	DateUtc smalldatetime not null,
	CounterId int not NULL,

	Identifier nvarchar(80) null,
	NetworkId int null,
	UserId int null,

	CONSTRAINT FK_StatsCounterHits_Counter
		FOREIGN KEY (CounterId)
		REFERENCES dbo.StatsCounters (Id),
	
	CONSTRAINT FK_StatsCounterHits_Network
		FOREIGN KEY (NetworkId)
		REFERENCES dbo.Networks (Id),
	
	CONSTRAINT FK_StatsCounterHits_User
		FOREIGN KEY (UserId)
		REFERENCES dbo.Users (Id),
)
GO



