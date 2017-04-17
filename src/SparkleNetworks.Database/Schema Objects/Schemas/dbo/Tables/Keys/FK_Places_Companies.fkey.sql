ALTER TABLE [dbo].[Places]
    ADD CONSTRAINT [FK_Places_Companies] FOREIGN KEY ([CompanyOwner]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

