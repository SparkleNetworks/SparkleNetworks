
CREATE TABLE [dbo].[Networks]
(
    [Id]              INT              IDENTITY (1, 1) NOT NULL,
    [Name]			  NVARCHAR (48)    NOT NULL,
    [DisplayName]	  NVARCHAR (80)    NOT NULL,
    [About]           nvarchar(4000)   NULL,
    [Color]			  nvarchar(7)      null,
    [Sector]          nvarchar (30)    NULL,
    [SiteUrl]         nvarchar (100)   NULL,
    [Address]         NVARCHAR (100)   NULL,
    [ZipCode]         NVARCHAR (10)    NULL,
    [City]            NVARCHAR (50)    NULL,
    [Country]         NVARCHAR (50)    NULL,
    [lat]			  FLOAT            NULL,
    [lon]			  FLOAT            NULL,
    [FoursquareId]	  NVARCHAR (30)    NULL,
    NetworkTypeId     int not null default 1,

    [TwitterUrl] NVARCHAR(100) NULL, 
    [FacebookUrl] NVARCHAR(100) NULL, 
    [BlogUrl] NVARCHAR(100) NULL, 
    
    DefaultCulture    varchar(10)      null default 'fr-FR',
    
    CONSTRAINT FK_Networks_Type FOREIGN KEY ([NetworkTypeId]) REFERENCES [dbo].[NetworkTypes] (Id),
)
