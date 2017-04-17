
CREATE TABLE [dbo].[Ads]
(
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [CategoryId]       INT            NOT NULL,
    [Date]             DATETIME       NOT NULL,
    [Title]            NVARCHAR (100) NOT NULL,
    [Message]          NVARCHAR(4000) NOT NULL,
    [Visibility]       BIT            NOT NULL,  -- OBSOLETE
    [UserId]           INT            NOT NULL,

	NetworkId          int      null,            -- NULL because added later
	Alias              NVARCHAR (100) NULL,      -- NULL because added later
	UpdateDateUtc      datetime null,

	IsValidated        bit      null,
	ValidationDateUtc  datetime null,
	ValidationUserId   int      null,
	IsOpen             bit      not null default 1,
	CloseDateUtc       datetime null,
	CloseUserId        int      null,

	PendingEditTitle   nvarchar(100)  null,
	PendingEditMessage nvarchar(4000) null,
	PendingEditDate    datetime       null,

    CONSTRAINT [PK_eura_Ads] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Ads_AdsCategories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[AdCategories] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Ads_Users]         FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT FK_Ads_NetworkId       FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
	CONSTRAINT FK_Ads_ValidationUser  FOREIGN KEY (ValidationUserId) REFERENCES dbo.Users (Id),
	CONSTRAINT FK_Ads_CloseUser       FOREIGN KEY (CloseUserId) REFERENCES dbo.Users (Id),
)
GO

CREATE UNIQUE INDEX UX_Ads_Alias ON dbo.Ads (Alias) INCLUDE (CategoryId, [Date], UpdateDateUtc, IsValidated, IsOpen)
GO

