
CREATE UNIQUE NONCLUSTERED INDEX [UX_Companies_Alias]
ON [dbo].Companies (Alias ASC)
INCLUDE ([NetworkId], [IsApproved], [IsEnabled]);

