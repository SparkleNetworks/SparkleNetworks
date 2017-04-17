ALTER TABLE [dbo].[Places]
    ADD CONSTRAINT [FK_Places_ParentPlace] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Places] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

