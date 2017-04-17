
CREATE TABLE dbo.EmailAddresses
(
    Id                   INT NOT NULL IDENTITY (1, 1),
    AccountPart          NVARCHAR (250)   NOT NULL,
    TagPart              NVARCHAR (250)   NULL,
    DomainPart           NVARCHAR (250)   NOT NULL,
    DateCreatedUtc       DATETIME         NOT NULL,
    DateConfirmedUtc     DATETIME         NULL,
    IsClosed             bit not null default 0,

    CONSTRAINT PK_dbo_EmailAddresses PRIMARY KEY (Id),
)
GO

CREATE UNIQUE INDEX UX_dbo_EmailAddresses_Email ON dbo.EmailAddresses (AccountPart, TagPart, DomainPart)
GO
