CREATE TABLE [dbo].[Universes] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (48) NOT NULL,
    [DisplayName] NVARCHAR (80) NOT NULL,
    [Status]      SMALLINT       NOT NULL
);

