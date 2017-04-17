
CREATE TABLE [dbo].[ApplyRequests]
(
    [Id]                    INT                 NOT NULL IDENTITY (1, 1),
    [Key]                   UNIQUEIDENTIFIER    NOT NULL,
    NetworkId               int                 not null,
    [Data]                  NVARCHAR(MAX)       NULL,
    [CompanyData]           NVARCHAR(MAX)       NULL,
    JoinCompanyId           INT                 NULL,
    [DateCreatedUtc]        DATETIME            NOT NULL,
    [DateSubmitedUtc]       DATETIME            NULL,
    [DateEmailConfirmedUtc] SMALLDATETIME       NULL,
    UserRemoteAddress       varchar(39)         null,
    DateRefusedUtc          SMALLDATETIME       NULL,
    DateAcceptedUtc         SMALLDATETIME       NULL,
    RefusedByUserId         int                 NULL,
    AcceptedByUserId        int                 NULL,
    CreatedUserId           INT                 NULL,
    CreatedCompanyId        INT                 NULL,
    RefusedRemark           NVARCHAR(4000)      NULL,
    DateDeletedUtc          SMALLDATETIME       NULL,
    DeletedByUserId         INT                 NULL,
    InvitedByUserId         INT                 NULL,
    DateInvitedUtc          SMALLDATETIME       NULL,
    ProcessData             NVARCHAR(MAX)       NULL,
    ImportedId              NVARCHAR (420)      NULL,

    CONSTRAINT PK_dbo_ApplyRequests PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT UC_dbo_ApplyRequests_Key UNIQUE ([Key]),
    CONSTRAINT FK_dbo_ApplyRequests_Network           FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
    CONSTRAINT FK_dbo_ApplyRequests_JoinCompany       FOREIGN KEY (JoinCompanyId) REFERENCES dbo.Companies (ID),
    CONSTRAINT FK_dbo_ApplyRequests_CreatedUser       FOREIGN KEY (CreatedUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT FK_dbo_ApplyRequests_CreatedCompany    FOREIGN KEY (CreatedCompanyId) REFERENCES dbo.Companies (ID),
    CONSTRAINT FK_dbo_ApplyRequests_AcceptedByUser    FOREIGN KEY (AcceptedByUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT FK_dbo_ApplyRequests_RefusedByUser     FOREIGN KEY (RefusedByUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT FK_dbo_ApplyRequests_DeletedByUser     FOREIGN KEY (DeletedByUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT FK_dbo_ApplyRequests_InvitedByUser     FOREIGN KEY (InvitedByUserId) REFERENCES dbo.Users (Id),
    CONSTRAINT CC_dbo_ApplyRequests_RefusedOrAccepted CHECK
    (
        DateRefusedUtc IS NULL AND DateAcceptedUtc IS NULL
        OR
        DateRefusedUtc IS NOT NULL AND DateAcceptedUtc IS NULL
        OR
        DateRefusedUtc IS NULL AND DateAcceptedUtc IS NOT NULL
    ),
    CONSTRAINT CC_dbo_ApplyRequests_AcceptedFields CHECK
    (
        DateAcceptedUtc IS NOT NULL AND CreatedUserId IS NOT NULL
        OR
        DateAcceptedUtc IS NULL AND CreatedUserId IS NULL
    ),
    CONSTRAINT CC_dbo_ApplyRequests_RefusedFields CHECK
    (
        DateRefusedUtc IS NOT NULL AND RefusedByUserId IS NOT NULL AND CreatedUserId IS NULL AND CreatedCompanyId IS NULL
        OR
        DateRefusedUtc IS NULL AND RefusedByUserId IS NULL AND CreatedUserId IS NULL AND CreatedCompanyId IS NULL
        OR
        DateRefusedUtc IS NULL AND RefusedByUserId IS NULL AND CreatedUserId IS NOT NULL AND CreatedCompanyId IS NOT NULL
        OR
        DateRefusedUtc IS NULL AND RefusedByUserId IS NULL AND CreatedUserId IS NOT NULL AND CreatedCompanyId IS NULL
    ),
    CONSTRAINT CC_dbo_ApplyRequests_DeletedFields CHECK
    (
        DateDeletedUtc IS NOT NULL AND DeletedByUserId IS NOT NULL
        OR
        DateDeletedUtc IS NULL AND DeletedByUserId IS NULL
    ),
    CONSTRAINT CC_dbo_ApplyRequests_InvitedFields CHECK
    (
        DateInvitedUtc IS NOT NULL AND InvitedByUserId IS NOT NULL
        OR
        DateInvitedUtc IS NULL AND InvitedByUserId IS NULL
    )
)
GO
