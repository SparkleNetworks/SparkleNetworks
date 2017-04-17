
CREATE TABLE [dbo].[ExchangeSkills] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]       INT            NOT NULL,
    [Date]            DATETIME       NOT NULL,
    [SkillId]         INT            NOT NULL,
    [Type]            TINYINT        NOT NULL,
    [Title]           NVARCHAR (100) NOT NULL,
    [Description]     NVARCHAR (4000)NOT NULL,
    [Status]          TINYINT        NOT NULL,
    [CreatedByUserId] INT            NOT NULL,
    [NetworkId]   int            not null,

    CONSTRAINT [PK_eura_ExchangeSkills] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExchangeSkills_CreatedBy] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ExchangeSkills_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ExchangeSkills_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ExchangeSkills_Skill] FOREIGN KEY ([SkillId]) REFERENCES [dbo].[Skills] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
