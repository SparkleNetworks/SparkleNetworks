﻿ALTER TABLE [dbo].[Pictures]
    ADD CONSTRAINT [FK_Pictures_Album] FOREIGN KEY ([AlbumId]) REFERENCES [dbo].[Albums] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

