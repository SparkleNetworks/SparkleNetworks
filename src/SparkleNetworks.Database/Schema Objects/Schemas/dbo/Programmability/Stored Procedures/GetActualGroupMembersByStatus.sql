
CREATE PROCEDURE dbo.GetActualGroupMembersByStatus
	@groupId int,
	@userId int,
	@status int = 0
AS

;WITH ids AS
(
	SELECT GroupId, UserId, max(id) Id, min(id) MinId, count(*) MembershipsCount
	FROM dbo.groupmembers
	WHERE
		(@groupId is null OR GroupId = @groupId)
	AND
		(@userId is null OR UserId = @userId)
	GROUP BY GroupId, UserId
)

select gm.*, d.MinId, d.MembershipsCount
from dbo.groupmembers gm
inner join ids d on d.Id = gm.id
inner join dbo.Groups g on gm.GroupId = g.Id and g.IsDeleted = 0
inner join dbo.Users u on u.Id = gm.UserId and u.NetworkAccessLevel > 0 and u.CompanyAccessLevel > 0 and u.IsEmailConfirmed = 1
inner join dbo.Companies c on c.ID = u.CompanyID
where gm.Accepted = @status and c.IsEnabled = 1
