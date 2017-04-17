
CREATE TABLE [dbo].[AchievementsUsers] (
    [AchievementId] INT      NOT NULL,
    [DateEarned]    DATETIME NOT NULL,
    [UserId]        INT      NOT NULL,

	CONSTRAINT [PK_AchievementsUsers] PRIMARY KEY CLUSTERED ([AchievementId] ASC, UserId ASC),
	CONSTRAINT [FK_AchievementsUsers_User]
		FOREIGN KEY ([UserId])
		REFERENCES [dbo].[Users] ([Id])
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [FK_AchievementsUsers_Achievement]
		FOREIGN KEY ([AchievementId])
		REFERENCES [dbo].[Achievements] ([Id])
		ON DELETE NO ACTION ON UPDATE NO ACTION,
)

