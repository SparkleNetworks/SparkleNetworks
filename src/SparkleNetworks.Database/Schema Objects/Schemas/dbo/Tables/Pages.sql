
CREATE TABLE [dbo].[Pages]
(
    [Id]               INT              NOT NULL IDENTITY(1, 1),
    NetworkId          int              not null,
    UniqueId           uniqueidentifier not null,
    DateCreatedUtc     datetime         NOT NULL,
    Alias              nvarchar(120)    NOT NULL,
    TextId             int              not null,

    IsEnabled          bit              not null default 1, -- Setting to false will disable dispaly.
    IsPublic           bit              not null default 0, -- Setting to true will allow anonymous users to see it.
    IsListed           bit              not null default 1, -- Setting to true will list the page in the pages main index.
    IsSearchEnginesIndexed bit          not null default 1, -- Allow search engines to index this page.
    IsReadOnly         bit              not null default 0, -- Prevent edits on this page.
    
    SecretCode         nvarchar(120)    null,               -- "Protect" page using a password.

    CONSTRAINT PK_dbo_Pages   PRIMARY KEY (Id),
    CONSTRAINT FK_dbo_Pages   FOREIGN KEY (TextId)    REFERENCES dbo.[Text]   (Id),
    CONSTRAINT FK_dbo_Network FOREIGN KEY (NetworkId) REFERENCES dbo.Networks (Id),
)
GO

CREATE UNIQUE INDEX UX_dbo_Pages_UniqueId ON dbo.Pages (NetworkId, UniqueId)
GO

CREATE UNIQUE INDEX UX_dbo_Pages_Alias ON dbo.Pages (NetworkId, Alias)
INCLUDE (TextId)
GO
