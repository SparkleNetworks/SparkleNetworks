
CREATE PROCEDURE [dbo].[GetEmailSendStatsByMinute]
    @dateFromUtc smalldatetime,
    @dateToUtc smalldatetime
AS

select [DateSentUtc] DateUtc, COUNT(Id) [Count]
from dbo.EmailMessages
where @dateFromUtc < [DateSentUtc] AND [DateSentUtc] < @dateToUtc
group by [DateSentUtc]
order by [DateSentUtc] desc
