
CREATE TABLE [dbo].[Albums] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (MAX) NOT NULL,
    [Visibility] INT            NOT NULL,
    [CompanyId]  INT            NULL,
    [EventId]    INT            NULL,
    [GroupId]    INT            NULL,
    [TeamId]     INT            NULL,
    [ProjectId]  INT            NULL,
    [PlaceId]    INT            NULL,
    [UserId]     INT            NOT NULL,

    CONSTRAINT [PK_eura_Albums] PRIMARY KEY CLUSTERED ([Id] ASC),
);

