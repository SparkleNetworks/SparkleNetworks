CREATE TABLE [dbo].[RequestsForProposal] (
    [Id]					INT				IDENTITY (1, 1) NOT NULL,
	[CompanySector]			NVARCHAR (MAX)	NOT NULL,
	[CompanyName]			NVARCHAR (MAX)	NOT NULL,
	[CompanyEmail]			NVARCHAR (MAX)	NOT NULL,
	[CompanyPhoneNumber]	NVARCHAR (MAX)	NULL,
	[Category]				NVARCHAR (MAX)	NOT NULL,
    [Description]			NVARCHAR(4000)	NOT NULL,
    [MinBudget]				INT				NULL,
    [MaxBudget]				INT				NULL,
    [Date]					DATETIME		NOT NULL,
    [Deadline]				DATETIME		NOT NULL,
	[NetworkId]   int            not null
);
