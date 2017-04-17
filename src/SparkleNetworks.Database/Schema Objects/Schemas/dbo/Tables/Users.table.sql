
CREATE TABLE [dbo].[Users] (
    Id                          INT              IDENTITY (1, 1) NOT NULL,
    UserId                      UNIQUEIDENTIFIER NOT NULL,  -- pointer to userid of aspnet_membership
    [Login]                     NVARCHAR (50)    NOT NULL,  -- DUPLICATION of username field of aspnet_membership
    FirstName                   NVARCHAR (100)   COLLATE French_CI_AI NOT NULL,
    LastName                    NVARCHAR (100)   COLLATE French_CI_AI NOT NULL,
    Gender                      INT              NULL,
    Birthday                    DATE             NULL,
    Picture                     NVARCHAR (100)   NULL,      -- obsolete
    Email                       NVARCHAR (100)   NULL,      -- DUPLICATION of email field of aspnet_membership
    PersonalEmail               NVARCHAR (100)   NULL,      -- should be removed in favor of UserProfileField
    --Phone                     NVARCHAR (100)   NULL,      -- removed in favor of UserProfileField
    CompanyID                   INT              NOT NULL,
    JobId                       INT              NULL,
    --Site                      NVARCHAR (120)   NULL,      -- removed in favor of UserProfileField
    --Twitter                   NVARCHAR (50)    NULL,      -- removed in favor of UserProfileField
    --City                      NVARCHAR (50)    NULL,      -- removed in favor of UserProfileField
    --ZipCode                   NVARCHAR (5)     NULL,      -- removed in favor of UserProfileField
    --About                     NVARCHAR(4000)   NULL,      -- removed in favor of UserProfileField
    --FavoriteQuotes            NVARCHAR(4000)   NULL,      -- removed in favor of UserProfileField
    --CurrentTarget             NVARCHAR(4000)   NULL,      -- removed in favor of UserProfileField
    --Contribution              NVARCHAR(4000)   NULL,      -- removed in favor of UserProfileField
    Score                       INT              NOT NULL,  -- obsolete?
    InvitationsLeft             INT              NOT NULL,  -- obsolete?
    RelationshipId              INT              NULL,
    ProjectId                   INT              NULL,      -- obsolete
    Superior                    UNIQUEIDENTIFIER NULL,      -- obsolete
    IsTeamMember                BIT              NULL,      -- obsolete?
    AccountClosed               BIT              NULL,      -- obsolete
    AccountRight                TINYINT          NULL,      -- obsolete
    CompanyAccessLevel          INT              NOT NULL,  -- 0=disabled, 1=User, 2=CommunityManager, 3=Administrator
    NetworkAccessLevel          INT              NOT NULL,  -- 0=disabled, 1⇒User, ...
    NetworkId                   int              not null,
    IsEmailConfirmed            bit              not null default 1,
    --ImportData                nvarchar(4000)   null,
    CreatedDateUtc              smalldatetime    null,
    AccountClosedDateUtc        smalldatetime    null,
    PersonalDataUpdateDateUtc   smalldatetime    null,      -- last date of personal data informations update
    Culture                     varchar(10)      null default 'fr-FR',
    Timezone                    nvarchar(180)    null default 'Romance Standard Time',
    LinkedInUserId              VARCHAR(12)      NULL,

    CONSTRAINT [PK_eura_Peoples] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_Users_Relationship] FOREIGN KEY ([RelationshipId]) REFERENCES [dbo].[Relationship] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Users_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Users_Jobs] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Jobs] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Users_Companies] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Users_aspnetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
