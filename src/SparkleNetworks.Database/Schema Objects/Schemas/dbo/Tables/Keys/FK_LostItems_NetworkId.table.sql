
ALTER TABLE [dbo].[LostItems]
    ADD CONSTRAINT [FK_LostItems_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
