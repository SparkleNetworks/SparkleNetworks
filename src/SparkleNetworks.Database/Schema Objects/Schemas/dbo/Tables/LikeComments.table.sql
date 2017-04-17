
CREATE TABLE [dbo].[LikeComments] (
    [TimelineItemCommentId] INT           NOT NULL,
    [UserId]                INT           NOT NULL,
    [FirstDateLikedUtc]     DATETIME      NULL,
    [IsLiked]               BIT           NULL,
    [WasInstantlyNotified]  BIT           NOT NULL DEFAULT 0,
    [DateReadUtc]           SMALLDATETIME NULL,

    CONSTRAINT [PK_eura_LikesComment] PRIMARY KEY CLUSTERED ([TimelineItemCommentId] ASC, [UserId] ASC),
    CONSTRAINT [FK_LikeComments_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_LikesComment_TimelineItemComments] FOREIGN KEY ([TimelineItemCommentId]) REFERENCES [dbo].[TimelineItemComments] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
