CREATE TABLE [dbo].[MenuPlanning] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [Date]      DATETIME NOT NULL,
    [CompanyId] INT      NOT NULL,
    [MenuId]    INT      NOT NULL,
    [PlaceId]   INT      NOT NULL,
    [Text]      NVARCHAR(4000) NULL
);

