﻿ALTER TABLE [dbo].[ListItems]
    ADD CONSTRAINT [FK_ListItems_Lists] FOREIGN KEY ([ListId]) REFERENCES [dbo].[Lists] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

