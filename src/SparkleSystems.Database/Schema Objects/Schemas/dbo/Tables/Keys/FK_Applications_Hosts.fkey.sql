ALTER TABLE [dbo].[Applications]
    ADD CONSTRAINT [FK_Applications_Hosts] FOREIGN KEY ([HostId]) REFERENCES [dbo].[Hosts] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

