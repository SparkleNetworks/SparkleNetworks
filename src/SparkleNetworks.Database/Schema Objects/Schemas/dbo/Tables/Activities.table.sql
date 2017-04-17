
CREATE TABLE dbo.Activities
(
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Type]      INT            NOT NULL, -- The type of activity.
    [Message]   NVARCHAR (MAX) NULL,	 -- Kinda obsolete thing but still used.
    [ProfileID] INT            NULL,	 -- A secondary user ID (someone that does something for the current user)
    [CompanyId] INT            NULL,	 -- ???
    [GroupId]   INT            NULL,	 -- 
    [EventId]   INT            NULL,	 -- 
    [Date]      DATETIME       NOT NULL, -- 
    [Displayed] BIT            NOT NULL, -- Indicates whether the item has been displayed.
    [UserId]    INT            NOT NULL, -- The MAIN USER
    AdId        INT            NULL,     --

    CONSTRAINT [PK_eura_Activities] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Activities_Companies]
        FOREIGN KEY ([CompanyId])
        REFERENCES [dbo].[Companies] ([ID])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Activities_User]
        FOREIGN KEY ([UserId])
        REFERENCES [dbo].[Users] ([Id])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT FK_Activities_Ad
        FOREIGN KEY (AdId)
        REFERENCES dbo.Ads (Id),

);

