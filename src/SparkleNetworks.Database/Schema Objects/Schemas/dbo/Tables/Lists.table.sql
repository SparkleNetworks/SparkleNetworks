CREATE TABLE [dbo].[Lists] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [OwnerType]       INT NOT NULL,
    [OwnerValue]      INT NOT NULL,
    [Focus]           BIT NOT NULL,
    [CreatedByUserId] INT NOT NULL
);

