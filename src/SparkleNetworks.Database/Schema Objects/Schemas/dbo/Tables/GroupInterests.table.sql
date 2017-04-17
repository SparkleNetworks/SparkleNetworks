
CREATE TABLE [dbo].[GroupInterests]
(
    [Id]					INT IDENTITY (1, 1) NOT NULL , 
    [GroupId]				INT					NOT NULL, 
    [InterestId]			INT					NOT NULL, 
    [DateCreatedUtc]		SMALLDATETIME		NOT NULL, 
    [CreatedByUserId]		int					NOT NULL,

    [DeletedDateUtc]		SMALLDATETIME		NULL,
    [DeletedByUserId]		INT					NULL,
    [DeleteReason]			TINYINT				NULL,

    CONSTRAINT [PK_dbo_GroupInterests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_dbo_GroupInterests_Group] FOREIGN KEY ([GroupId]) REFERENCES [Groups]([Id]),
    CONSTRAINT [FK_dbo_GroupInterests_Skill] FOREIGN KEY ([InterestId]) REFERENCES [Interests]([Id]),
    CONSTRAINT [FK_dbo_GroupInterests_CreatedBy] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_dbo_GroupInterests_DeletedBy] FOREIGN KEY (DeletedByUserId) REFERENCES [Users](Id),
    CONSTRAINT [CC_dbo_GroupInterests_DeleteCheck] CHECK 
    (
        (DeletedDateUtc is null AND DeletedByUserId is null AND DeleteReason is null) 
        OR 
        (DeletedDateUtc is not null AND DeletedByUserId is not null AND DeleteReason is not null)
    )
);
