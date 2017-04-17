﻿ALTER TABLE [dbo].[SeekFriends]
    ADD CONSTRAINT [PK_SeekFriends] PRIMARY KEY CLUSTERED ([SeekerId] ASC, [TargetId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

