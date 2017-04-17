CREATE UNIQUE NONCLUSTERED INDEX [IX_Logs_ApplicationId]
ON [dbo].[ApplicationConfigValues]
(
    [ApplicationId] ASC,
    [ConfigKeyId] ASC,
    [Index] ASC
)
WITH
(
    ALLOW_PAGE_LOCKS = ON,
    ALLOW_ROW_LOCKS = ON,
    PAD_INDEX = OFF,
    SORT_IN_TEMPDB = OFF,
    DROP_EXISTING = OFF,
    IGNORE_DUP_KEY = OFF,
    STATISTICS_NORECOMPUTE = OFF,
    ONLINE = OFF,
    MAXDOP = 0)
ON [PRIMARY];

