CREATE TABLE [dbo].[UserSkills] (
    [Id]      INT      IDENTITY (1, 1) NOT NULL,
    [SkillId] INT      NOT NULL,
    [Date]    DATETIME NOT NULL,
    [UserId]  INT      NOT NULL
);

