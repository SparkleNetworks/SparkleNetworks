
ALTER TABLE [dbo].[Numbers]
    ADD CONSTRAINT [FK_Numbers_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
