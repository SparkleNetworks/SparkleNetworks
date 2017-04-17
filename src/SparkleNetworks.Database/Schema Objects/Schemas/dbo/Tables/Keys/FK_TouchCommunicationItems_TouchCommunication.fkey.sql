ALTER TABLE [dbo].[TouchCommunicationItems]
    ADD CONSTRAINT [FK_TouchCommunicationItems_TouchCommunication] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[TouchCommunications] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

