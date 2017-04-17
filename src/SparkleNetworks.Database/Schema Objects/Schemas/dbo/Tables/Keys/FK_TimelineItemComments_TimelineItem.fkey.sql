ALTER TABLE [dbo].[TimelineItemComments]
    ADD CONSTRAINT [FK_TimelineItemComments_TimelineItem] FOREIGN KEY ([TimelineItemId]) REFERENCES [dbo].[TimelineItems] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

