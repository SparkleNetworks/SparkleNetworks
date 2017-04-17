ALTER TABLE [dbo].[Groups]
    ADD CONSTRAINT [DF_eura_Groups_NotificationFrequency] DEFAULT ((2)) FOR [NotificationFrequency];

