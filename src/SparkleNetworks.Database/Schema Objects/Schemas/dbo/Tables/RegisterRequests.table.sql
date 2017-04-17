
CREATE TABLE [dbo].[RegisterRequests]
(
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [EmailAccountPart]    NVARCHAR (100)   NOT NULL,
    [EmailTagPart]        NVARCHAR (100)   NULL,
    [EmailDomain]         NVARCHAR (100)   NOT NULL,
    [CompanyName]         nvarchar (100)   NULL,
    [CompanyId]           int              NULL,
    [DateCreatedUtc]      DATETIME         NOT NULL,
    [DateUpdatedUtc]      DATETIME         NULL,
    [Status]              smallint         NOT NULL DEFAULT(0),
    [ReplyUserId]         int              NULL,
    [ValidatedByUserId]   int              NULL,
    [NetworkId]           int              not null,
    [Code]                UNIQUEIDENTIFIER NULL DEFAULT NEWID(),
    AcceptedInvitationId  int              null,
    --[Code]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- uncomment after full deployment

    CONSTRAINT [PK_RegisterRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RegisterRequests_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RegisterRequests_ReplyUserId] FOREIGN KEY ([ReplyUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RegisterRequests_ValidatedByUserId] FOREIGN KEY ([ValidatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RegisterRequests_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].Companies ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RegisterRequests_AcceptedInvitation] FOREIGN KEY (AcceptedInvitationId) REFERENCES [dbo].Invited ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT CC_dbo_RegisterRequests_CompanyNameOrId CHECK
    (
        (CompanyName IS NULL AND CompanyId IS NOT NULL)
        OR
        (CompanyName IS NOT NULL AND CompanyId IS NULL)
    ),
    --CONSTRAINT UC_dbo_RegisterRequests_Code UNIQUE (Code), -- uncomment after full deployment
    CONSTRAINT UC_dbo_RegisterRequests_EmailAddress UNIQUE (EmailAccountPart, EmailTagPart, EmailDomain),
);
