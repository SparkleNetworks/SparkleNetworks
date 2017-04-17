
CREATE TABLE dbo.Users
(
    Id                         INT              IDENTITY (1, 1) NOT NULL,
    DisplayName                NVARCHAR (250)   NOT NULL,
    Firstname                  NVARCHAR (250)   NULL,
    Lastname                   NVARCHAR (250)   NULL,
    Country                    CHAR (2)         NULL,
    Culture                    VARCHAR (5)      NULL,
    Timezone                   NVARCHAR (180)   NULL,
    DateCreatedUtc             DATETIME         NOT NULL,
    [Status]                   SMALLINT         NOT NULL DEFAULT 0,
	PrimaryEmailAddressId      int              not null,

    CONSTRAINT PK_dbo_Users PRIMARY KEY (Id),
	CONSTRAINT FK_dbo_Users_PrimaryEmailAddress FOREIGN KEY (PrimaryEmailAddressId) REFERENCES dbo.EmailAddresses (Id),
)
GO
