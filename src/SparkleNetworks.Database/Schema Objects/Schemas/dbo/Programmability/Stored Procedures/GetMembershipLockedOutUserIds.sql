
CREATE PROCEDURE [dbo].[GetMembershipLockedOutUserIds]
	@networkId int
AS

select u.Id
from dbo.Users u
inner join dbo.aspnet_Membership m on u.UserId = m.UserId and m.IsLockedOut = 1
where u.NetworkId = @networkId
