--
-- STORED PROCEDURE
--     [dbo].[GetUsersConversations]
--
-- DESCRIPTION
--     Groups messages by conversation for a user.
--     A conversation contains 1 to 4 rows.
--
-- PARAMETERS
--     userId
--
-- RETURN VALUE
--
-- PROGRAMMING NOTES
--     N/A.
--

CREATE PROCEDURE [dbo].[GetUsersConversations]
	@userId int,
	@networkId int
AS

select
	M.FromUserId, M.ToUserId, M.Displayed,
	MAX(M.CreateDate) CreateDate, COUNT(*) [Count], MAX(M.Id) MaxId
from dbo.Messages M
left join dbo.Users UF ON UF.Id = M.FromUserId
left join dbo.Users UT ON UT.Id = M.ToUserId
where 
(
	(M.FromUserId = @userId AND UT.NetworkId = @networkId)
	OR
	(M.ToUserId = @userId   AND UF.NetworkId = @networkId)
)
group by M.FromUserId, M.ToUserId, M.Displayed
order by CreateDate desc
