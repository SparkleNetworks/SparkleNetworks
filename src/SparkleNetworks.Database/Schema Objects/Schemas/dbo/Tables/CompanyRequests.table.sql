
CREATE TABLE [dbo].CompanyRequests
(
    [Id]          INT            IDENTITY (1, 1) NOT NULL,

    -- details
    [Name]        NVARCHAR (100) NOT NULL,
    [Alias]       NVARCHAR (100) NOT NULL,
    [Website]     NVARCHAR (100) NULL,
    [Phone]       NVARCHAR (50)  NULL,
    [Email]       NVARCHAR (100) NULL,
    [Building]    NVARCHAR (50)  NULL,
    [Floor]       INT            NULL,
    [Access]      NVARCHAR (30)  NULL,
    [Baseline]    NVARCHAR(4000) NULL,
    [About]       NVARCHAR(4000) NULL,
    [EmailDomain] NVARCHAR (50)  NULL,
    [CategoryId]  SMALLINT       NOT NULL,
    [IndoorId]    SMALLINT       NOT NULL,
    [NetworkId]   int            not null,
    AdminEmails   nvarchar(1200) null,
    OtherEmails   nvarchar(1200) null,

    -- artifacts
    UniqueId          uniqueidentifier not null,
    CreatedDateUtc    datetime         not null default '2014-01-17 17:00:00',

    -- approve / reject
    CompanyId         int             null, -- if approved, else stays null
    Approved          bit             null, -- if approved/rejected, else stays null
    ApprovedReason    nvarchar(400)   null, -- if rejected, else stays null
    ClosedDateUtc     datetime        null, -- if approved/rejected, else stays null
    ClosedByUserId    int             null, -- if approved/rejected, else stays null
    RejectedTimes     smallint        not null default 0,

    -- blocked
    BlockedDateUtc    datetime        null, -- if rejected and blocked, else stays null
    BlockedByUserId   int             null, -- if rejected and blocked, else stays null
    BlockedReason     nvarchar(400)   null, -- if rejected and blocked, else stays null

    CONSTRAINT PK_dbo_CompanyRequests PRIMARY KEY (Id ASC),
    CONSTRAINT FK_dbo_CompanyRequests_Network FOREIGN KEY ([NetworkId]) REFERENCES dbo.Networks (Id),
    CONSTRAINT FK_dbo_CompanyRequests_Company FOREIGN KEY (CompanyId)   REFERENCES dbo.Companies (ID),
    CONSTRAINT FK_dbo_CompanyRequests_ApprovedByUser FOREIGN KEY (ClosedByUserId)  REFERENCES dbo.Users (Id),
    CONSTRAINT FK_dbo_CompanyRequests_BlockedByUser  FOREIGN KEY (BlockedByUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT FK_dbo_CompanyRequests_Category FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[CompanyCategories] (Id),
    CONSTRAINT UC_dbo_CompanyRequests_UniqueId UNIQUE (UniqueId),
)
