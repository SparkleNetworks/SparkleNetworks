ALTER TABLE [dbo].[PollAnswers]
    ADD CONSTRAINT [FK_PollAnswers_PollChoices] FOREIGN KEY ([ChoiceId]) REFERENCES [dbo].[PollChoices] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;

