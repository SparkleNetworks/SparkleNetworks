﻿ALTER TABLE [dbo].[TeamMembers]
    ADD CONSTRAINT [FK_TeamsMembers_Team] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

