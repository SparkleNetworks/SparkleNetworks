
CREATE PROCEDURE [dbo].[GetUserPresenceStats]
    @NetworkId int
AS


declare @UserPresenceAverageDays float
declare @UserPresenceMaxDays float

select @UserPresenceAverageDays = avg(GroupByUser.Days), @UserPresenceMaxDays = MAX(GroupByUser.Days)
from (
    select P.UserId, CAST(COUNT(P.Day) as float) Days
    from dbo.UserPresences P
    inner join dbo.Users U on U.NetworkId = @NetworkId and U.Id = P.UserId
    group by P.UserId
) GroupByUser

declare @UserPresenceAverageUsers float
declare @UserPresenceMaxUsers float

select @UserPresenceAverageUsers = avg(GroupByDay.Users), @UserPresenceMaxUsers = MAX(GroupByDay.Users)
from (
    select P.Day, CAST(COUNT(P.UserId) as float) Users
    from dbo.UserPresences P
    inner join dbo.Users U on U.NetworkId = @NetworkId and U.Id = P.UserId
    group by P.Day
) GroupByDay

declare @TotalDays int
select @TotalDays = COUNT(distinct Day) from dbo.UserPresences

declare @TotalUsers int
select @TotalUsers = COUNT(distinct UserId) from dbo.UserPresences

select 
    @UserPresenceAverageDays UserPresenceAverageDays,
    @UserPresenceMaxDays UserPresenceMaxDays,
    @UserPresenceAverageUsers PerDayPresenceAverageUsers,
    @UserPresenceMaxUsers PerDayPresenceMaxUsers,
    @TotalDays TotalDays,
    @TotalUsers TotalUsers

