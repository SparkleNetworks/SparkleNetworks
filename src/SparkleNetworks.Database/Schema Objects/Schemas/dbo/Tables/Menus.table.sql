CREATE TABLE [dbo].[Menus] (
    [Id]        INT   IDENTITY (1, 1) NOT NULL,
    [CompanyId] INT   NOT NULL,
    [Text]      NVARCHAR(4000) NOT NULL
);

