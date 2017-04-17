ALTER TABLE [dbo].[Teams]
    ADD CONSTRAINT [FK_Teams_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

