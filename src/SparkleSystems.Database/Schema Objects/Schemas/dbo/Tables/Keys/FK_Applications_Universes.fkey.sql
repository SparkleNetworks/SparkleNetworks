ALTER TABLE [dbo].[Applications]
    ADD CONSTRAINT [FK_Applications_Universes] FOREIGN KEY ([UniverseId]) REFERENCES [dbo].[Universes] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

