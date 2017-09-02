
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class EmailMessagesRepository : BaseNetworkRepositoryInt<EmailMessage>, IEmailMessagesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public EmailMessagesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.EmailMessages)
        {
        }

        public IList<GetEmailSendStatsByMinute_Result> GetSendStatsByMinute(DateTime dateFromUtc, DateTime dateToUtc)
        {
            return this.Context.GetEmailSendStatsByMinute(dateFromUtc, dateToUtc)
                .ToList();
        }

        public IList<GetEmailSendStatsByHour_Result> GetSendStatsByHour(DateTime dateFromUtc, DateTime dateToUtc)
        {
            return this.Context.GetEmailSendStatsByHour(dateFromUtc, dateToUtc)
                .ToList();
        }

        public IList<GetEmailSendStatsByHour_Result> GetSendStatsByDay(DateTime dateFromUtc, DateTime dateToUtc)
        {
            return this.Context.GetEmailSendStatsByDay(dateFromUtc, dateToUtc)
                .ToList();
        }
    }
}
