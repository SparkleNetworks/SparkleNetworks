
CREATE TABLE [dbo].[CompanyNews] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Text]            NVARCHAR (MAX) NULL,
    [Subject]         NVARCHAR (255) NULL,
    [CreateDate]      DATE           NULL,
    [CompanyId]       INT            NULL,
    [CreatedByUserId] INT            NOT NULL,

    CONSTRAINT [PK_eura_CompaniesNews] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyNews_User] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_CompaniesNews_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);

