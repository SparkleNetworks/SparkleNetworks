CREATE PROCEDURE [dbo].[GetTimelineSkillsListIdForCount]
	@networkId int
AS

SELECT S.*
FROM dbo.TimelineItemSkills S
LEFT JOIN dbo.TimelineItems T on T.Id = S.TimelineItemId AND S.DeletedByUserId is null
WHERE T.NetworkId = @networkId AND S.DeletedDateUtc IS NULL
