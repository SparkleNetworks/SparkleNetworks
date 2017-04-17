
ALTER TABLE [dbo].[Places]
    ADD CONSTRAINT [FK_Places_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
