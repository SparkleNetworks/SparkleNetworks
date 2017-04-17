
CREATE NONCLUSTERED INDEX [IX_Users_MostFrequent]
ON [dbo].[Users]
(
    [Id] ASC,
    [Login] ASC,
    [CompanyID] ASC,
    [JobId] ASC,
    [CompanyAccessLevel] ASC,
    [NetworkAccessLevel] ASC, 
    [NetworkId] ASC,
    [IsEmailConfirmed] ASC
)
INCLUDE	([FirstName], [LastName], [Gender], [Picture], [Email], [AccountClosed], [CreatedDateUtc], [Culture], [Timezone])
GO
