CREATE TABLE [dbo].[SeekFriends] (
    [HasAccepted]    BIT  NULL,
    [ExpirationDate] DATE NULL,
    [CreateDate]     DATE NULL,
    [SeekerId]       INT  NOT NULL,
    [TargetId]       INT  NOT NULL
);

