
CREATE PROCEDURE [dbo].[CountActiveSubscriptionsInDateRange]
	@begin datetime,-- = (dateadd(month, -1, getutcdate())),
	@end   datetime --= getutcdate()
AS

;with cte as
(
	select s.Id, s.DateBeginUtc, s.DateEndUtc, s.TemplateId
	from dbo.Subscriptions s
	inner join dbo.SubscriptionTemplates t on t.Id = s.TemplateId
	where 
	(
		(DateEndUtc is not null and DateBeginUtc <= @begin and @begin <= DateEndUtc)
		or 
		(DateEndUtc is null and DateBeginUtc <= @begin)
	)
	and (IsPaid = 1 and (AppliesToCompanyId is not null or AppliesToUserId is not null))
)

select TemplateId, COUNT(id) SubscriptionCount
from cte
group by TemplateId

