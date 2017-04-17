ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [DF_eura_Peoples_NetworkAccessLevel] DEFAULT ((1)) FOR [NetworkAccessLevel];

