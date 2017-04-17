
CREATE TABLE dbo.Hints
(
    Id            INT NOT NULL IDENTITY (1, 1),
    Alias         nvarchar(200) not null,
    Title         nvarchar(200) null,
    Location      nvarchar(200) null,
    [Description] nvarchar(2000) null,
    IsEnabled     bit not null,
    HintTypeId    int not null,
    NetworkId     int null,
    
    CONSTRAINT PK_dbo_Hints PRIMARY KEY (Id),
    CONSTRAINT UC_dbo_Hints_Alias UNIQUE (Alias),
    CONSTRAINT FK_dbo_Hints_Network FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
)
GO

