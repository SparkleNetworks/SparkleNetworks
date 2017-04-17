ALTER TABLE [dbo].[Applications]
    ADD CONSTRAINT [FK_Applications_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

