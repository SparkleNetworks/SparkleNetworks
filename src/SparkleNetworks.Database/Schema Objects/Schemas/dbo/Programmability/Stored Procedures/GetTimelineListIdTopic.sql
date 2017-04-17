CREATE PROCEDURE [dbo].[GetTimelineListIdTopic]
	@networkId int,
	@dateMax DateTime,
	@skillId int
AS

SELECT TOP 40
	T.Id
FROM dbo.TimelineItems T
LEFT JOIN TimelineItemSkills S on S.TimelineItemId = T.Id
WHERE 
	T.NetworkId = @networkId
	AND S.DeletedDateUtc is null
	AND T.CreateDate < @dateMax
	AND S.SkillId = @skillId
ORDER BY T.CreateDate DESC, T.Id ASC
