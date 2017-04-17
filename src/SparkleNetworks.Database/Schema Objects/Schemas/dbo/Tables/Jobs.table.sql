
CREATE TABLE [dbo].[Jobs] (
    [Id]      INT IDENTITY (1, 1) NOT NULL,
    [Libelle] NVARCHAR(80) NULL,
    [Alias]   NVARCHAR(80) NULL,
    GroupName NVARCHAR(80) NULL,

	CONSTRAINT [UC_dbo_Jobs_Alias] UNIQUE ([Alias])
);

