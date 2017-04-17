
CREATE TABLE [dbo].[TimelineItemSkills]
(
    [Id]			        INT				    IDENTITY (1, 1) NOT NULL,
    [TimelineItemId]        INT				    NOT NULL,
    [SkillId]		        INT				    NOT NULL,
    [DateUtc]		        SMALLDATETIME		NOT NULL,
    [CreatedByUserId]       int					NOT NULL,
    
    [DeletedDateUtc]        SMALLDATETIME		NULL,
    [DeletedByUserId]       INT					NULL,
    [DeleteReason]          TINYINT             NULL,
    
    CONSTRAINT PK_dbo_TimelineItemSkills PRIMARY KEY (Id),
    CONSTRAINT FK_dbo_TimelineItemSkills_TimelineItem FOREIGN KEY (TimelineItemId) REFERENCES dbo.TimelineItems (Id),
    CONSTRAINT FK_dbo_TimelineItemSkills_Skill FOREIGN KEY (SkillId) REFERENCES dbo.Skills (Id),
    CONSTRAINT FK_dbo_TimelineItemSkills_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT UC_dbo_TimelineItemSkills_TimelineItem_Skill UNIQUE (TimelineItemId, SkillId),
    CONSTRAINT FK_dbo_TimelineItemSkills_DeletedBy FOREIGN KEY (DeletedByUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT CC_dbo_TimelineItemSkills_DeleteCheck CHECK
    (
        (DeletedDateUtc is null AND DeletedByUserId is null)
        OR
        (DeletedDateUtc is not null AND DeletedByUserId is not null)
    )
)


