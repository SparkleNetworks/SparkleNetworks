
CREATE TABLE [dbo].[EventPublicMembers]
(
    [Id]				 INT			  NOT NULL IDENTITY (1, 1),
    [EventId]			 INT			  NOT NULL,
    [State]				 INT			  NOT NULL,
    [FirstName]          NVARCHAR (100)   NOT NULL,
    [LastName]           NVARCHAR (100)   NOT NULL,
    [Email]              NVARCHAR (100)   NOT NULL,
    [Company]            NVARCHAR (100)   NULL,
    [Job]				 NVARCHAR (100)   NULL,
    [Phone]              NVARCHAR (100)   NULL,
    DateCreatedUtc       smalldatetime    null,
    DateUpdatedUtc       smalldatetime    null,
    RemoteAddress        varchar(39)      null,

    CONSTRAINT [PK_EventPublicMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_dbo_EventPublicMembers_Event] FOREIGN KEY ([EventId]) REFERENCES dbo.Events(Id),
)
