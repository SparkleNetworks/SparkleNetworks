CREATE TABLE [dbo].[AchievementsCompanies] (
    [AchievementId] INT      NOT NULL,
    [CompanyId]     INT      NOT NULL,
    [DateEarned]    DATETIME NOT NULL,
    
	CONSTRAINT [PK_AchievementsCompanies] PRIMARY KEY CLUSTERED ([AchievementId] ASC, CompanyId ASC),
    CONSTRAINT [FK_AchievementsCompanies_Achievement]
        FOREIGN KEY ([AchievementId])
        REFERENCES [dbo].[Achievements] ([Id])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_AchievementsCompanies_Company]
		FOREIGN KEY ([CompanyId])
		REFERENCES [dbo].[Companies] ([Id])
		ON DELETE NO ACTION ON UPDATE NO ACTION,
);

