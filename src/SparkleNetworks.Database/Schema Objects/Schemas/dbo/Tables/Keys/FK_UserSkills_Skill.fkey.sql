﻿ALTER TABLE [dbo].[UserSkills]
    ADD CONSTRAINT [FK_UserSkills_Skill] FOREIGN KEY ([SkillId]) REFERENCES [dbo].[Skills] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

