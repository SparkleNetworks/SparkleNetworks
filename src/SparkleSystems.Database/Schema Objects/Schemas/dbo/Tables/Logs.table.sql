CREATE TABLE [dbo].[Logs] (
    [Id]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [UtcDateTime]        DATETIME       NOT NULL,
    [Type]               TINYINT        NOT NULL,
    [ApplicationId]      INT            NOT NULL,
    [ApplicationVersion] NVARCHAR (20)  NOT NULL,
    [Path]               NVARCHAR (255) NULL,
    [RemoteClient]       NVARCHAR (50)  NULL,
    [Identity]           NVARCHAR (50)  NULL,
    [Error]              TINYINT        NOT NULL,
    [Data]               TEXT           NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The type of log event', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs', @level2type = N'COLUMN', @level2name = N'Type';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The application path (controller/action, window/panel/command, page/method).', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs', @level2type = N'COLUMN', @level2name = N'Path';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The remote client address (IPv4, IPv6).', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs', @level2type = N'COLUMN', @level2name = N'RemoteClient';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'A user credential if available.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs', @level2type = N'COLUMN', @level2name = N'Identity';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'A status code.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs', @level2type = N'COLUMN', @level2name = N'Error';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Optional debugging data.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Logs', @level2type = N'COLUMN', @level2name = N'Data';

