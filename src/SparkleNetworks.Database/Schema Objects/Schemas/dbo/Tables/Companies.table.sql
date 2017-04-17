
CREATE TABLE [dbo].[Companies] (
    [ID]                          INT            IDENTITY (1, 1) NOT NULL,
    [Name]                        NVARCHAR (100) COLLATE French_CI_AI NOT NULL,
    [Alias]                       NVARCHAR (100) NOT NULL,
    [Website]                     NVARCHAR (100) NULL,
    [Phone]                       NVARCHAR (50)  NULL,
    [Email]                       NVARCHAR (100) NULL,
    [Baseline]                    NVARCHAR(200)  NULL,
    [About]                       NTEXT          NULL,
    [EmailDomain]                 NVARCHAR (50)  NULL,
    [Facebook]                    NVARCHAR (100) NULL,
    [Twitter]                     NVARCHAR (100) NULL,
    [AngelList]                   NVARCHAR (100) NULL,
    [CategoryId]                  SMALLINT       NOT NULL DEFAULT 3,
    [NetworkId]                   INT            NOT NULL,
    [IsApproved]                  BIT            NOT NULL DEFAULT 1,
    [CreatedDateUtc]              DATETIME       NOT NULL DEFAULT '2012-02-02 17:0:0',
    [ApprovedDateUtc]             DATETIME       NULL,
    [IsEnabled]                   BIT            NOT NULL DEFAULT 1,
    [IsEnabledFirstChangeDateUtc] SMALLDATETIME  NULL,
    [IsEnabledFirstChangeUserId]  INT            NULL,
    [IsEnabledLastChangeDateUtc]  SMALLDATETIME  NULL,
    [IsEnabledLastChangeUserId]   INT            NULL,
    [IsEnabledRemark]             NVARCHAR(4000) NULL,
    ImportedId                    NVARCHAR (420) NULL,

    CONSTRAINT [PK_eura_Companies] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT FK_Companies_Category    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[CompanyCategories] (Id),
    CONSTRAINT [FK_Companies_NetworkId] FOREIGN KEY ([NetworkId])  REFERENCES [dbo].[Networks] ([Id]),

    CONSTRAINT [FK_dbo_Companies_UserFirstChange] FOREIGN KEY ([IsEnabledFirstChangeUserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_dbo_Companies_UserLastChange]  FOREIGN KEY ([IsEnabledLastChangeUserId])  REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [C_dbo_Companies_IsEnabledFirstChangeCheck] CHECK
    (
        ([IsEnabledFirstChangeDateUtc] IS NULL AND [IsEnabledFirstChangeUserId] IS NULL)
        OR
        ([IsEnabledFirstChangeDateUtc] IS NOT NULL AND [IsEnabledFirstChangeUserId] IS NOT NULL)
    ),
    CONSTRAINT [C_dbo_Companies_IsEnabledLastChangeCheck] CHECK
    (
        ([IsEnabledLastChangeDateUtc] IS NULL AND [IsEnabledLastChangeUserId] IS NULL)
        OR
        ([IsEnabledLastChangeDateUtc] IS NOT NULL AND [IsEnabledLastChangeUserId] IS NOT NULL)
    ),
);
