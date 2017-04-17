
CREATE TABLE [dbo].[Devices] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [DeviceId]            UNIQUEIDENTIFIER NOT NULL,
    [Name]                NVARCHAR (150)   NULL,
    [FirstStartDateUtc]   DATETIME         NOT NULL,
    [LastActivityDateUtc] DATETIME         NOT NULL,
    [Version]             NVARCHAR (20)    NOT NULL,
    [Type]                NVARCHAR (150)   NULL,
    [DefaultLayout]       NVARCHAR (50)    NULL,
    [DefaultLayoutData]   NTEXT            NULL,
    [PlaceId]             INT              NULL,
    [NetworkId]   int            not null,

    CONSTRAINT [PK_Devices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Devices_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
