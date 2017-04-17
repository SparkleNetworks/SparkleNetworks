
CREATE TABLE [dbo].[CompanyRequestMessages]
(
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    CompanyRequestId  int                 not null,
    DateUtc           datetime            not null,
    IsMessageFromCompany  bit             not null,

    -- when message is from requester (not authenticated) to approver -- IsMessageFromCompany=1
    NewReplyEmail         nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS null,

    -- when message is from approver (authenticated) to requester	  -- IsMessageFromCompany=0
    [FromUserId]          INT             NULL,
    [FromUserName]        NVARCHAR (100)  NULL,
    ToEmailAddress        nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS null,
    
    Content               nvarchar (4000) not null,
    DisplayDateUtc        smalldatetime   null,

    CONSTRAINT PK_dbo_CompanyRequestMessages PRIMARY KEY (Id ASC),
    CONSTRAINT PK_dbo_CompanyRequestMessages_CompanyRequest FOREIGN KEY (CompanyRequestId) REFERENCES dbo.CompanyRequests (Id),
    CONSTRAINT PK_dbo_CompanyRequestMessages_FromUser FOREIGN KEY (FromUserId) REFERENCES dbo.Users (Id),
)
