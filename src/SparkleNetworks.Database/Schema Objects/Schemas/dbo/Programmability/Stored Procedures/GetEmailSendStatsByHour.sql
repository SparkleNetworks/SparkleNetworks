﻿
CREATE PROCEDURE [dbo].[GetEmailSendStatsByHour]
    @dateFromUtc smalldatetime,
    @dateToUtc smalldatetime
AS

select
    (
        CAST(DATEPART(year, DateSentUtc) AS varchar)+
        '-'+
        CAST(DATEPART(month, DateSentUtc) AS varchar)+
        '-'+
        CAST(DATEPART(day, DateSentUtc) AS varchar)+
        ' '+
        CAST(DATEPART(hour, DateSentUtc) AS varchar)+
        ':00:00'
    ) DateUtc, 
    COUNT(id) [Count]
from EmailMessages
where @dateFromUtc < [DateSentUtc] AND [DateSentUtc] < @dateToUtc
group by DATEPART(year, DateSentUtc),DATEPART(month, DateSentUtc),DATEPART(day, DateSentUtc),DATEPART(hour, DateSentUtc)
order by DATEPART(year, DateSentUtc),DATEPART(month, DateSentUtc),DATEPART(day, DateSentUtc),DATEPART(hour, DateSentUtc) desc
