ALTER TABLE [dbo].[MenuPlanning]
    ADD CONSTRAINT [FK_MenuPlanning_Menus] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;

