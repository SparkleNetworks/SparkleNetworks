﻿ALTER TABLE [dbo].[ProjectMembers]
    ADD CONSTRAINT [PK_eura_ProjectsMembers] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
