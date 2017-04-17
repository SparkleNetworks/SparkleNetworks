ALTER TABLE [dbo].[SocialNetworkCompanySubscriptions]
    ADD CONSTRAINT [FK_SocialNetworkCompanySubscriptions_Company] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID])
	ON DELETE NO ACTION ON UPDATE NO ACTION;

