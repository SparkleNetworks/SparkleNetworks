
CREATE TABLE [dbo].[Applications]
(
    [Id]         INT     IDENTITY (1, 1) NOT NULL,
    [ProductId]  INT     NOT NULL,
    [UniverseId] INT     NOT NULL,
    [HostId]     INT     NOT NULL,
    [Status]     SMALLINT NOT NULL,
)
GO

CREATE UNIQUE INDEX UX_Applications_Parts
ON [dbo].[Applications] (ProductId, UniverseId, HostId)
GO
