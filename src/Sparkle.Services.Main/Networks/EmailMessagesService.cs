
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Objects;
    using SrkToolkit.Domain;

    public class EmailMessagesService : ServiceBase, IEmailMessagesService
    {
        [DebuggerStepThrough]
        internal EmailMessagesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public void Insert(EmailMessage message)
        {
            this.SetNetwork(message);
            this.Repo.EmailMessages.Insert(message);
        }

        public IList<EmailStatGroup> GetSendStatsByMinute(DateTime dateFromUtc, DateTime dateToUtc)
        {
            return this.Repo.EmailMessages.GetSendStatsByMinute(dateFromUtc, dateToUtc)
                .Select(x => new EmailStatGroup
                {
                    DateUtc = x.DateUtc,
                    Count = x.Count ?? 0,
                })
                .ToList();
        }

        public IList<EmailStatGroup> GetSendStatsByHour(DateTime dateFromUtc, DateTime dateToUtc)
        {
            return this.Repo.EmailMessages.GetSendStatsByHour(dateFromUtc, dateToUtc)
                .Select(x => new EmailStatGroup
                {
                    DateUtc = DateTime.Parse(x.DateUtc).AsUtc(),
                    Count = x.Count ?? 0,
                })
                .OrderByDescending(x => x.DateUtc)
                .ToList();
        }

        public IList<EmailStatGroup> GetSendStatsByDay(DateTime dateFromUtc, DateTime dateToUtc)
        {
            return this.Repo.EmailMessages.GetSendStatsByDay(dateFromUtc, dateToUtc)
                .Select(x => new EmailStatGroup
                {
                    DateUtc = DateTime.Parse(x.DateUtc).AsUtc(),
                    Count = x.Count ?? 0,
                })
                .OrderByDescending(x => x.DateUtc)
                .ToList();
        }
    }
}
