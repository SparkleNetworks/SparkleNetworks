﻿ALTER TABLE [dbo].[PollAnswers]
    ADD CONSTRAINT [FK_PollAnswers_Polls] FOREIGN KEY ([PollId]) REFERENCES [dbo].[Polls] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

