
namespace Sparkle.Services.Networks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Objects;

    public interface IEmailMessagesService
    {
        void Insert(EmailMessage message);

        IList<EmailStatGroup> GetSendStatsByMinute(DateTime dateFromUtc, DateTime dateToUtc);
        IList<EmailStatGroup> GetSendStatsByHour(DateTime dateFromUtc, DateTime dateToUtc);
        IList<EmailStatGroup> GetSendStatsByDay(DateTime dateFromUtc, DateTime dateToUtc);
    }
}
