
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Repository]
    public interface IEmailMessagesRepository : IBaseNetworkRepository<EmailMessage, int>
    {
        IList<GetEmailSendStatsByMinute_Result> GetSendStatsByMinute(DateTime dateFromUtc, DateTime dateToUtc);
        IList<GetEmailSendStatsByHour_Result> GetSendStatsByHour(DateTime dateFromUtc, DateTime dateToUtc);
        IList<GetEmailSendStatsByHour_Result> GetSendStatsByDay(DateTime dateFromUtc, DateTime dateToUtc);
    }
}
