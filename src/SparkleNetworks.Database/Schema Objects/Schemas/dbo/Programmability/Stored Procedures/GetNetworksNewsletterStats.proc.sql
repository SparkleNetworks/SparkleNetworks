CREATE PROCEDURE [dbo].[GetNetworksNewsletterStats]
  @NetworkId int
AS

select
  C.Category,
  C.Name,
  H.Identifier,
  COUNT(distinct H.userid) UserHits,
  (COUNT(NULLIF(0, H.userid)) - COUNT(H.userid)) AnonymousHits
from [dbo].[StatsCounterHits] H
inner join [dbo].[StatsCounters] C
  on C.Id = H.CounterId
where C.Name IN ('WeeklyNewsletter', 'DailyNewsletter') AND C.Category IN ('Follow', 'Display', 'Send') AND H.NetworkId = @NetworkId
group by C.Category, C.Name, Identifier
order by Identifier
