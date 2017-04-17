ALTER TABLE [dbo].[UsersVisits]
    ADD CONSTRAINT [FK_UsersVisits_Profile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

