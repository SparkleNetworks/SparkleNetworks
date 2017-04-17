CREATE PROCEDURE [dbo].[GetLastFiveRegistrants]
	@networkId int
AS

SELECT TOP 5 *
FROM dbo.TimelineItems T
WHERE T.NetworkId = @networkId AND T.ItemType = 1 AND T.DeleteReason IS NULL
ORDER BY T.Id DESC
