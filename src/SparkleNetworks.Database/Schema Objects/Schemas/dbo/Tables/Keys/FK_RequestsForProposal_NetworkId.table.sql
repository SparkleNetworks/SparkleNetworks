
ALTER TABLE [dbo].[RequestsForProposal]
    ADD CONSTRAINT [FK_RequestsForProposal_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
