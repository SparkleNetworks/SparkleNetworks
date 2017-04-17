ALTER TABLE [dbo].[UserInterests]
    ADD CONSTRAINT [FK_UserInterests_Interest] FOREIGN KEY ([InterestId]) REFERENCES [dbo].[Interests] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

