
CREATE TABLE [dbo].[CompanyCategories]
(
    Id            SMALLINT      IDENTITY (1, 1) NOT NULL,
    Name          nvarchar(150) COLLATE French_CI_AI not null,
    Alias         NVARCHAR(150) NOT NULL,
    NetworkId     INT           NOT NULL,
    KnownCategory TINYINT       NOT NULL DEFAULT 0,
    IsDefault     BIT           NOT NULL DEFAULT 0,

    CONSTRAINT PK_CompanyCategories PRIMARY KEY (Id),
    CONSTRAINT UC_dbo_CompanyCategories UNIQUE ([Alias], [NetworkId]),
    CONSTRAINT FK_CompanyCategories_Networks FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]),
)
