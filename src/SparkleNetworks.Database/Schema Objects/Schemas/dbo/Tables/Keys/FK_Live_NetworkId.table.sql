﻿
ALTER TABLE [dbo].[Live]
    ADD CONSTRAINT [FK_Live_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Networks] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
