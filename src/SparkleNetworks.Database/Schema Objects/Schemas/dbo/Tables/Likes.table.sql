
CREATE TABLE [dbo].[Likes] (
    [TimelineItemId]        INT           NOT NULL,
    [UserId]                INT           NOT NULL,
    [FirstDateLikedUtc]     DATETIME      NULL,
    [IsLiked]               BIT           NULL,
    [WasInstantlyNotified]  BIT           NOT NULL DEFAULT 0,
    [DateReadUtc]           SMALLDATETIME NULL,

    CONSTRAINT [PK_eura_Likes] PRIMARY KEY CLUSTERED ([TimelineItemId] ASC, [UserId] ASC),
    CONSTRAINT [FK_Likes_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_Likes_TimelineItems] FOREIGN KEY ([TimelineItemId]) REFERENCES [dbo].[TimelineItems] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);

