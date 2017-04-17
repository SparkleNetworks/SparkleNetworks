CREATE TABLE [dbo].[Teams] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [CompanyId]   INT           NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    [Description] NVARCHAR(4000) NULL,
    [Date]        DATETIME      NOT NULL,
    [OwnerType]   INT           NOT NULL,
    [OwnerValue]  INT           NOT NULL
);

