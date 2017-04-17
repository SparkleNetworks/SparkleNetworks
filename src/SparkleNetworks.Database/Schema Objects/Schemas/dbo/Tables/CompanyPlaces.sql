CREATE TABLE [dbo].[CompanyPlaces]
(
	[Id]                INT         NOT NULL IDENTITY (1, 1),
	[CompanyId]         INT         NOT NULL,
	[PlaceId]           INT         NOT NULL,
	[DateCreatedUtc]    DATETIME    NOT NULL,
	[CreatedByUserId]   INT         NOT NULL,
	[DateDeletedUtc]    DATETIME    NULL,
	[DeletedByUserId]   INT         NULL,

	CONSTRAINT [PK_dbo_CompanyPlaces] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_dbo_CompanyPlaces_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([Id]),
	CONSTRAINT [FK_dbo_CompanyPlaces_Places] FOREIGN KEY ([PlaceId]) REFERENCES [dbo].[Places] ([Id]),
	CONSTRAINT [UC_dbo_CompanyPlaces] UNIQUE ([CompanyId], [PlaceId]),
	CONSTRAINT [CC_dbo_CompanyPlaces_Delete] CHECK
	(
		[DateDeletedUtc] IS NULL AND [DeletedByUserId] IS NULL
		OR
		[DateDeletedUtc] IS NOT NULL AND [DeletedByUserId] IS NOT NULL
	),
)
