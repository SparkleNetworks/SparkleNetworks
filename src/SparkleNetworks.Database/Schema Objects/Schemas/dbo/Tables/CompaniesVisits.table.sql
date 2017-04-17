CREATE TABLE [dbo].[CompaniesVisits] (
    [Id]        INT     IDENTITY (1, 1) NOT NULL,
    [UserId]    INT     NOT NULL,
    [Date]      DATE    NOT NULL,
    [CompanyId] INT     NOT NULL,
    [ViewCount] TINYINT NOT NULL,

    CONSTRAINT [PK_CompaniesVisits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompaniesVisits_Company]
        FOREIGN KEY ([CompanyId])
        REFERENCES [dbo].[Companies] ([ID])
        ON DELETE CASCADE ON UPDATE NO ACTION,
    CONSTRAINT [FK_CompaniesVisits_User]
        FOREIGN KEY ([UserId])
        REFERENCES [dbo].[Users] ([Id])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
);

