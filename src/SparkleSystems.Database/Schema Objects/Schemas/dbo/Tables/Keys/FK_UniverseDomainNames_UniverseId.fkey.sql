ALTER TABLE [dbo].[UniverseDomainNames]
    ADD CONSTRAINT [FK_UniverseDomainNames_UniverseId]
    FOREIGN KEY ([UniverseId])
    REFERENCES [dbo].[Universes] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

