CREATE TABLE [dbo].[PlaceHistory] (
    [Id]            INT      IDENTITY (1, 1) NOT NULL,
    [PlaceId]       INT      NOT NULL,
    [PlaceParentId] INT      NOT NULL,
    [Day]           DATE     NOT NULL,
    [Hour]          TIME (7) NOT NULL,
    [Activity]      INT      NULL,
    [UserId]        INT      NOT NULL
);

