ALTER TABLE [dbo].[Places]
    ADD CONSTRAINT [FK_Places_PlaceCategories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[PlaceCategories] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

