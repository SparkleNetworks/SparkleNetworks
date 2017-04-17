CREATE PROCEDURE [dbo].[GetEventStatsPerMonth]
	@networkId int
AS

SELECT 
	(CAST(YEAR(E.DateEvent) AS VARCHAR(4)) + '-' + CAST(MONTH(E.DateEvent) AS VARCHAR(2)) + '-01') AS Month,
	COUNT(distinct E.id) AS Events,
	COUNT(EM.UserId) AS Members
FROM [dbo].[Events] E
LEFT JOIN EventMembers EM ON EM.EventId = E.Id AND EM.State = 1
WHERE E.NetworkId = @networkId
GROUP BY CAST(YEAR(E.DateEvent) AS VARCHAR(4)) + '-' + CAST(MONTH(E.DateEvent) AS VARCHAR(2))
