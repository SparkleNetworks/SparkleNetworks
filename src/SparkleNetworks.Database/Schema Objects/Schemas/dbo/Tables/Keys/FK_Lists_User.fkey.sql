﻿ALTER TABLE [dbo].[Lists]
    ADD CONSTRAINT [FK_Lists_User] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

