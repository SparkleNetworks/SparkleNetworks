
--CREATE VIEW [dbo].[GroupMembersView]
--AS

--with ids as
--(
--	select groupid, userid, max(id) Id, min(id) MinId, count(*) MembershipsCount from dbo.groupmembers
--	group by groupid, userid
--)

--select gm.*, d.MinId, d.MembershipsCount
--from dbo.groupmembers gm
--inner join ids d on d.Id = gm.id
