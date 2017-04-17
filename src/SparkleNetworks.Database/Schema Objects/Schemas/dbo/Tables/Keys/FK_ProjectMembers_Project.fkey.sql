ALTER TABLE [dbo].[ProjectMembers]
    ADD CONSTRAINT [FK_ProjectMembers_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

