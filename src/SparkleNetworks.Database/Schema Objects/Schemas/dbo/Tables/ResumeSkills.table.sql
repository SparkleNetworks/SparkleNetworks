CREATE TABLE [dbo].[ResumeSkills] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [ResumeId]  INT      NOT NULL,
    [SkillId]   INT      NOT NULL,
    [DateUtc]   DATETIME NOT NULL,
);

