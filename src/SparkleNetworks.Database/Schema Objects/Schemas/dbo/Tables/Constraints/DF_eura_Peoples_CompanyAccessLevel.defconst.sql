ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [DF_eura_Peoples_CompanyAccessLevel] DEFAULT ((1)) FOR [CompanyAccessLevel];

