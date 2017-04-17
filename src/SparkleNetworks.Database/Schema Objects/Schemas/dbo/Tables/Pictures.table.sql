CREATE TABLE [dbo].[Pictures] (
    [Id]      INT      IDENTITY (1, 1) NOT NULL,
    [AlbumId] INT			NOT NULL,
    [Date]    DATETIME		NOT NULL,
    [Comment] NVARCHAR(4000) NULL,
    [UserId]  INT			NOT NULL
);

