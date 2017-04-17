ALTER TABLE [dbo].[TimelineItemComments]
    ADD CONSTRAINT [FK_TimelineItemComments_User] FOREIGN KEY ([PostedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

