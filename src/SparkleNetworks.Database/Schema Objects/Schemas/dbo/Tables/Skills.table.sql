
CREATE TABLE [dbo].[Skills] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [ParentId]        INT           NOT NULL,
    [TagName]         NVARCHAR (50) COLLATE French_CI_AI NOT NULL,
    [Date]            DATETIME      NOT NULL,
    [CreatedByUserId] INT           NOT NULL,

	CONSTRAINT [UC_Skills_TagName] UNIQUE ([TagName]),
);

