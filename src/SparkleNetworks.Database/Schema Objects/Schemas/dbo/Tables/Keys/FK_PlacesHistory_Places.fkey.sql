ALTER TABLE [dbo].[PlaceHistory]
    ADD CONSTRAINT [FK_PlacesHistory_Places] FOREIGN KEY ([PlaceId]) REFERENCES [dbo].[Places] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

