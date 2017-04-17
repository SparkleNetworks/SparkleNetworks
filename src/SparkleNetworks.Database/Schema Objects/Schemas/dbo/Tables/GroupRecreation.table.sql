
CREATE TABLE [dbo].[GroupRecreations]
(
    [Id]					INT IDENTITY (1, 1) NOT NULL , 
    [GroupId]				INT					NOT NULL, 
    [RecreationId]			INT					NOT NULL, 
    [DateCreatedUtc]		SMALLDATETIME		NOT NULL, 
    [CreatedByUserId]		int					NOT NULL,

    [DeletedDateUtc]		SMALLDATETIME		NULL,
    [DeletedByUserId]		INT					NULL,
    [DeleteReason]			TINYINT				NULL,

    CONSTRAINT [PK_dbo_GroupRecreations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_dbo_GroupRecreations_Group] FOREIGN KEY ([GroupId]) REFERENCES [Groups]([Id]),
    CONSTRAINT [FK_dbo_GroupRecreations_Skill] FOREIGN KEY ([RecreationId]) REFERENCES [Recreations]([Id]),
    CONSTRAINT [FK_dbo_GroupRecreations_CreatedBy] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_dbo_GroupRecreations_DeletedBy] FOREIGN KEY (DeletedByUserId) REFERENCES [Users](Id),
    CONSTRAINT [CC_dbo_GroupRecreations_DeleteCheck] CHECK 
    (
        (DeletedDateUtc is null AND DeletedByUserId is null AND DeleteReason is null) 
        OR 
        (DeletedDateUtc is not null AND DeletedByUserId is not null AND DeleteReason is not null)
    )
);
