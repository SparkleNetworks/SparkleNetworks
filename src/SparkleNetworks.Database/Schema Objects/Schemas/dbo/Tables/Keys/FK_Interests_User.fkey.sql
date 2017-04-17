ALTER TABLE [dbo].[Interests]
    ADD CONSTRAINT [FK_Interests_User] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

