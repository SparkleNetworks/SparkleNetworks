
CREATE PROCEDURE dbo.GetActualGroupMembers
	@groupId int,
	@userId int,
	@networkId int
AS

;WITH ids AS
(
	SELECT GroupId, UserId, max(id) Id, min(id) MinId, count(*) MembershipsCount
	FROM dbo.GroupMembers
	WHERE
		(@groupId is null OR GroupId = @groupId)
	AND
		(@userId is null OR UserId = @userId)
	GROUP BY GroupId, UserId
)

SELECT gm.*, d.MinId, d.MembershipsCount
FROM dbo.GroupMembers gm
INNER JOIN ids d ON d.Id = gm.Id
INNER JOIN dbo.Groups g ON gm.GroupId = g.Id and g.IsDeleted = 0
WHERE g.NetworkId = @networkId
