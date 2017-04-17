
CREATE TABLE [dbo].[ProfileFields]
(
    [Id]              INT                 NOT NULL,           -- No identity! Ids from 1 to 1000 are reserved for "Known fields". Extra fields can be added starting at Id 1001.
    [Name]            NVARCHAR(60)        NOT NULL,           --
    ApplyToUsers      bit                 not null default 0, -- Does this apply to users?
    ApplyToCompanies  bit                 not null default 0, -- Does this apply to companies?
    Rules             nvarchar(4000)      null,               -- JSON data here
    --[Type]            nvarchar(80)        null,               -- consider System.String when null

    CONSTRAINT PK_dbo_ProfileFields PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT UC_dbo_ProfileFields_UniqueName UNIQUE (Name),
)
