CREATE TABLE [dbo].[Projects] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [IsPrivate]       BIT           NULL,
    [Name]            NVARCHAR (50) NULL,
    [Description]     NVARCHAR(4000) NULL,
    [OwnerType]       INT           NOT NULL,
    [OwnerValue]      INT           NOT NULL,
    [CreatedByUserId] INT           NOT NULL
);

