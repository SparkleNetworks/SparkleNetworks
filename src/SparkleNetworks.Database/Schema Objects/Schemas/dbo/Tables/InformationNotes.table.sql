
CREATE TABLE [dbo].[InformationNotes] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [UserId]         INT            NOT NULL,
    [Name]           NVARCHAR (200) NOT NULL,
    [Description]    NVARCHAR(4000) NOT NULL,
    [StartDateUtc]   DATETIME       NOT NULL,
    [EndDateUtc]     DATETIME       NOT NULL,
    [NetworkId]      int            not null,
    -- TODO: DateCreatedUtc DATETIME NOT NULL,

    CONSTRAINT [PK_InformationNotes_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InformationNotes_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_InformationNotes_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
