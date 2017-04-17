
CREATE TABLE [dbo].[DeviceConfiguration]
(
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [Key]				  NVARCHAR (100)   NOT NULL,
    [Value]				  NVARCHAR (4000)  NOT NULL,
    [NetworkId]   int            not null,

    CONSTRAINT [PK_DeviceConfiguration] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_DeviceConfiguration_Key] UNIQUE ([Key], NetworkId),
    CONSTRAINT [FK_DeviceConfiguration_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
