
CREATE TABLE [dbo].[UserEmailChangeRequests]
(
    [Id]						INT				IDENTITY(1, 1) NOT NULL,
    [NetworkId]					INT				NOT NULL,

    [PreviousEmailAccountPart]	NVARCHAR(100)	NOT NULL,
    [PreviousEmailTagPart]		NVARCHAR(100)	NULL,
    [PreviousEmailDomainPart]	NVARCHAR(100)	NOT NULL,

    [NewEmailAccountPart]		NVARCHAR(100)	NOT NULL,
    [NewEmailTagPart]			NVARCHAR(100)	NULL,
    [NewEmailDomainPart]		NVARCHAR(100)	NOT NULL,

    [UserId]					INT				NOT NULL,
    [CreatedByUserId]			INT				NOT NULL,

    [Status]					INT				NOT NULL,
    [PreviousEmailForbidden]	INT				NOT NULL,

    [EmailChangeRemark]			NVARCHAR(4000)	NULL,
    [CreateDateUtc]				DATETIME		NOT NULL,
    [ValidateDateUtc]			DATETIME		NULL,
    
    CONSTRAINT [PK_dbo_UserEmailChangeRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserEmailChangeRequests_ConcernedUser] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_UserEmailChangeRequests_ActingUser] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_UserEmailChangeRequests_Networks] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
