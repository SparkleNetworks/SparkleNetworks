﻿ALTER TABLE [dbo].[UserSettings]
    ADD CONSTRAINT [PK_UserSettings]
	PRIMARY KEY CLUSTERED ([UserId] ASC, [Key] ASC)
	WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

	