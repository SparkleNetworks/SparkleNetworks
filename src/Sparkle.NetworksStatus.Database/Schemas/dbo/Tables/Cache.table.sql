CREATE TABLE [dbo].[Cache]
(
	[Id]                INT             IDENTITY (1, 1) NOT NULL,
	[Type]              TINYINT         NOT NULL,
	[Name]              NVARCHAR(140)   NOT NULL,
	[Value]             NVARCHAR(4000)  NOT NULL,
	[DateCreatedUtc]    SMALLDATETIME   NOT NULL,

	CONSTRAINT [PK_Cache_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
)
