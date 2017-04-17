
CREATE TABLE [dbo].[Messages] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [Text]              NVARCHAR(4000)  NULL,
    [CreateDate]        DATETIME        NOT NULL,
    [Subject]           NVARCHAR (255)  NULL,
    [Archived]          BIT             NULL,
    [Displayed]         BIT             NOT NULL,
    [FromUserId]        INT             NOT NULL,
    [ToUserId]          INT             NOT NULL,
    [Source]            tinyint         NOT NULL DEFAULT 0,
    [InboundEmailId]    INT             NULL,
);
