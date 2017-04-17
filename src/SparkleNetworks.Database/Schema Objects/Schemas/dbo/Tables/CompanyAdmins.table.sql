
CREATE TABLE [dbo].[CompanyAdmins] (
    [Id]        INT      IDENTITY (1, 1) NOT NULL,
    [CompanyId] INT      NOT NULL,
    [Date]      DATETIME NOT NULL,
    [UserId]    INT      NOT NULL,

    
    CONSTRAINT [PK_eura_CompaniesAdmin] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyAdmins_Companies]
        FOREIGN KEY ([CompanyId])
        REFERENCES [dbo].[Companies] ([ID])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_CompanyAdmins_User]
        FOREIGN KEY ([UserId])
        REFERENCES [dbo].[Users] ([Id])
        ON DELETE NO ACTION ON UPDATE NO ACTION,
    
);

