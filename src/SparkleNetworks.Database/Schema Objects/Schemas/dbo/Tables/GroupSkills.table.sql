
CREATE TABLE [dbo].[GroupSkills]
(
    [Id]					INT IDENTITY (1, 1) NOT NULL , 
    [GroupId]				INT					NOT NULL, 
    [SkillId]				INT					NOT NULL, 
    [DateCreatedUtc]		SMALLDATETIME		NOT NULL, 
    [CreatedByUserId]		int					NOT NULL,

    [DeleteReason]			TINYINT            NULL,
    [DeletedDateUtc]		SMALLDATETIME		NULL,
    [DeletedByUserId]		INT					NULL,

    CONSTRAINT [PK_dbo_GroupSkills] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_dbo_GroupSkills_Group] FOREIGN KEY ([GroupId]) REFERENCES [Groups]([Id]),
    CONSTRAINT [FK_dbo_GroupSkills_Skill] FOREIGN KEY ([SkillId]) REFERENCES [Skills]([Id]),
    CONSTRAINT [FK_dbo_GroupSkills_CreatedBy] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_dbo_GroupSkills_DeletedBy] FOREIGN KEY (DeletedByUserId) REFERENCES [Users](Id),
    CONSTRAINT [CC_dbo_GroupSkills_DeleteCheck] CHECK 
    (
        (DeletedDateUtc is null AND DeletedByUserId is null AND DeleteReason is null)
        OR
        (DeletedDateUtc is not null AND DeletedByUserId is not null AND DeleteReason is not null)
    )
)

GO

CREATE UNIQUE INDEX [UX_dbo_GroupSkills_Group_Skill]
ON [dbo].[GroupSkills] (GroupId , SkillId)
GO
