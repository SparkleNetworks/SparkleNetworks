
ALTER TABLE [dbo].[TouchCommunications]
ADD CONSTRAINT [FK_TouchCommunications_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
