
CREATE TABLE [dbo].[ApiKeys]
(
    [Id]               INT              IDENTITY(1, 1) NOT NULL,
    DateCreatedUtc     datetime         NOT NULL,
    [Key]              varchar(100)     NOT NULL,
    [Secret]           varchar(100)     NOT NULL,
    IsEnabled          bit              NOT NULL,
    Name               nvarchar(120)    NOT NULL,
    Description        nvarchar(1200)   NULL,
    Roles              nvarchar(1200)   NULL,

    CONSTRAINT PK_ApiKeys PRIMARY KEY (Id),
)
GO

CREATE UNIQUE INDEX UX_ApiKeys_Key ON dbo.ApiKeys ([Key]) INCLUDE([Secret], IsEnabled)
GO
