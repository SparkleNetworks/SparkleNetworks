
CREATE TABLE [dbo].[CompanySkills] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [CompanyId] INT      NOT NULL,
    [SkillId]   INT      NOT NULL,
    [Date]      DATETIME NOT NULL,

    CONSTRAINT [PK_eura_CompaniesSkills] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanySkills_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_CompanySkills_Skill] FOREIGN KEY ([SkillId]) REFERENCES [dbo].[Skills] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);

