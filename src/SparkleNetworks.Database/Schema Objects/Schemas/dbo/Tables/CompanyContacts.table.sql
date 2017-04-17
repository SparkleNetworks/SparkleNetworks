
CREATE TABLE [dbo].[CompanyContacts] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Type]			   INT            NOT NULL,
    [Date]			   DATETIME       NOT NULL,
    [ToCompanyId]      INT            NULL,
    [ToUserEmail]      NVARCHAR (200) NULL,
    [FromCompanyId]    INT            NULL,
    [FromCompanyName]  NVARCHAR (100) NULL,
    [FromUserId]       INT            NULL,
    [FromUserName]     NVARCHAR (100) NULL,
    [FromUserEmail]    NVARCHAR (200) NULL,
    [Message]          NVARCHAR (4000) NOT NULL,
    [IsRead]           BIT            NOT NULL DEFAULT 0,

    CONSTRAINT [PK_CompanyContacts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyContacts_FromUserId] FOREIGN KEY ([FromUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_CompanyContacts_ToCompanyId] FOREIGN KEY ([ToCompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_CompanyContacts_FromCompanyId] FOREIGN KEY ([FromCompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);
