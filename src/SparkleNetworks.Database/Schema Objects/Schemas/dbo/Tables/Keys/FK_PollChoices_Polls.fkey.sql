﻿ALTER TABLE [dbo].[PollChoices]
    ADD CONSTRAINT [FK_PollChoices_Polls] FOREIGN KEY ([PollId]) REFERENCES [dbo].[Polls] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;

