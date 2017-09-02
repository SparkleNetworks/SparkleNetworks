
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Objects;
    using Neutral = Sparkle.Entities.Networks.Neutral;

    public class StatsCountersService : ServiceBase, IStatsCountersService
    {
        private readonly List<Neutral.StatsCounter> counters = new List<Neutral.StatsCounter>();
        private readonly ReaderWriterLock countersLock = new ReaderWriterLock();

        [DebuggerStepThrough]
        internal StatsCountersService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public void SetupWeeklyNewsletterCounters()
        {
            var display = this.GetCounter(
                Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Display.Category,
                Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Display.Name);
            if (display == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Display.Category,
                    Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Display.Name);
            }

            var click = this.GetCounter(
                Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Follow.Category,
                Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Follow.Name);
            if (click == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Follow.Category,
                    Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Follow.Name);
            }

            var links = this.GetCounter(
                Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Links.Category,
                Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Links.Name);
            if (links == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Links.Category,
                    Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Links.Name);
            }

            var send = this.GetCounter(
                Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Send.Category,
                Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Send.Name);
            if (send == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Send.Category,
                    Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Send.Name);
            }
        }

        public void SetupDailyNewsletterCounters()
        {
            var display = this.GetCounter(
                Neutral.StatsCounter.WellKnown.DailyNewsletter.Display.Category,
                Neutral.StatsCounter.WellKnown.DailyNewsletter.Display.Name);
            if (display == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.DailyNewsletter.Display.Category,
                    Neutral.StatsCounter.WellKnown.DailyNewsletter.Display.Name);
            }

            var click = this.GetCounter(
                Neutral.StatsCounter.WellKnown.DailyNewsletter.Follow.Category,
                Neutral.StatsCounter.WellKnown.DailyNewsletter.Follow.Name);
            if (click == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.DailyNewsletter.Follow.Category,
                    Neutral.StatsCounter.WellKnown.DailyNewsletter.Follow.Name);
            }

            var links = this.GetCounter(
                Neutral.StatsCounter.WellKnown.DailyNewsletter.Links.Category,
                Neutral.StatsCounter.WellKnown.DailyNewsletter.Links.Name);
            if (links == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.DailyNewsletter.Links.Category,
                    Neutral.StatsCounter.WellKnown.DailyNewsletter.Links.Name);
            }

            var send = this.GetCounter(
                Neutral.StatsCounter.WellKnown.DailyNewsletter.Send.Category,
                Neutral.StatsCounter.WellKnown.DailyNewsletter.Send.Name);
            if (send == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.DailyNewsletter.Send.Category,
                    Neutral.StatsCounter.WellKnown.DailyNewsletter.Send.Name);
            }
        }

        public void SetupAffiliateCounters()
        {
            var display = this.GetCounter(
                Neutral.StatsCounter.WellKnown.Affiliates.Display.Category,
                Neutral.StatsCounter.WellKnown.Affiliates.Display.Name);
            if (display == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.Affiliates.Display.Category,
                    Neutral.StatsCounter.WellKnown.Affiliates.Display.Name);
            }

            var listDisplay = this.GetCounter(
                Neutral.StatsCounter.WellKnown.Affiliates.ListDisplay.Category,
                Neutral.StatsCounter.WellKnown.Affiliates.ListDisplay.Name);
            if (listDisplay == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.Affiliates.ListDisplay.Category,
                    Neutral.StatsCounter.WellKnown.Affiliates.ListDisplay.Name);
            }

            var follow = this.GetCounter(
                Neutral.StatsCounter.WellKnown.Affiliates.Follow.Category,
                Neutral.StatsCounter.WellKnown.Affiliates.Follow.Name);
            if (display == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.Affiliates.Follow.Category,
                    Neutral.StatsCounter.WellKnown.Affiliates.Follow.Name);
            }
        }

        public void SetupPartnerResourceCounters()
        {
            var display = this.GetCounter(
                Neutral.StatsCounter.WellKnown.PartnerResources.Display.Category,
                Neutral.StatsCounter.WellKnown.PartnerResources.Display.Name);
            if (display == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.PartnerResources.Display.Category,
                    Neutral.StatsCounter.WellKnown.PartnerResources.Display.Name);
            }

            var listDisplay = this.GetCounter(
                Neutral.StatsCounter.WellKnown.PartnerResources.ListDisplay.Category,
                Neutral.StatsCounter.WellKnown.PartnerResources.ListDisplay.Name);
            if (listDisplay == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.PartnerResources.ListDisplay.Category,
                    Neutral.StatsCounter.WellKnown.PartnerResources.ListDisplay.Name);
            }

            var follow = this.GetCounter(
                Neutral.StatsCounter.WellKnown.PartnerResources.Follow.Category,
                Neutral.StatsCounter.WellKnown.PartnerResources.Follow.Name);
            if (follow == null)
            {
                this.CreateCounter(
                    Neutral.StatsCounter.WellKnown.PartnerResources.Follow.Category,
                    Neutral.StatsCounter.WellKnown.PartnerResources.Follow.Name);
            }
        }

        public Neutral.StatsCounter GetCounter(int id)
        {
            Neutral.StatsCounter ct = null;
            this.countersLock.AcquireReaderLock(1000);
            try
            {
                ct = this.counters.SingleOrDefault(c => c.Id == id);
                if (ct == null)
                {
                    var entity = this.Repo.StatsCounters.GetById(id);

                    if (entity != null)
                    {
                        ct = Neutral.StatsCounter.FromEntity(entity);

                        var up = this.countersLock.UpgradeToWriterLock(1000);
                        try
                        {
                            this.counters.Add(ct);
                        }
                        finally
                        {
                            this.countersLock.DowngradeFromWriterLock(ref up);
                        }
                    }
                }
            }
            finally
            {
                this.countersLock.ReleaseReaderLock();
            }

            return ct;
        }

        public Neutral.StatsCounter GetCounter(string category, string name)
        {
            Neutral.StatsCounter ct = null;
            this.countersLock.AcquireReaderLock(1000);
            try
            {
                ct = this.counters.SingleOrDefault(c => c.Name == name && c.Category == category);
                if (ct == null)
                {
                    var entity = this.Repo.StatsCounters.GetCounter(category, name);

                    if (entity != null)
                    {
                        ct = Neutral.StatsCounter.FromEntity(entity);

                        var up = this.countersLock.UpgradeToWriterLock(1000);
                        try
                        {
                            this.counters.Add(ct);
                        }
                        finally
                        {
                            this.countersLock.DowngradeFromWriterLock(ref up);
                        }
                    }
                }
            }
            finally
            {
                this.countersLock.ReleaseReaderLock();
            }

            return ct;
        }

        public Neutral.StatsCounter CreateCounter(string category, string name)
        {
            var counter = new StatsCounter
            {
                Category = category,
                Name = name,
            };
            this.Repo.StatsCounters.Insert(counter);
            return new Neutral.StatsCounter
            {
                Id = counter.Id,
                Category = counter.Category,
                Name = counter.Name,
            };
        }

        public StatsCounterHitLink GetTrackingCode(string category, string name, string identifier = null, int? userId = null, int? networkId = null)
        {
            var counter = this.GetCounter(category, name);
            return new StatsCounterHitLink(counter, identifier, userId, networkId);
        }

        public StatsCounterHit Hit(string category, string name, string identifier = null, int? userId = null, int? networkId = null)
        {
            var counter = this.GetCounter(category, name);
            var hit = new StatsCounterHit
            {
                CounterId = counter.Id,
                DateUtc = DateTime.UtcNow,
                NetworkId = networkId,
                UserId = userId,
                Identifier = identifier,
            };
            this.Repo.StatsCounterHits.Insert(hit);
            return hit;
        }

        public StatsCounterHit Hit(string trackingCode)
        {
            StatsCounterHitLink obj;
            try
            {
                obj = StatsCounterHitLink.FromEncryptedJson(trackingCode);
            }
            catch (FormatException ex)
            {
                this.Services.Logger.Error("StatsCountersService.Hit(string)", ErrorLevel.Input, "FormatException parsing trackingCode \"" + trackingCode + "\"" + ex.Message);
                throw;
            }

            var counter = this.GetCounter(obj.Id);
            ////if (counter.Name != obj.Name || obj.Category != counter.Category)
            ////    counter = this.GetCounter(obj.Category, obj.Name);

            var hit = new StatsCounterHit
            {
                CounterId = counter.Id,
                DateUtc = DateTime.UtcNow,
                NetworkId = obj.NetworkId,
                UserId = obj.UserId != 0 ? obj.UserId : null,
                Identifier = obj.Identifier,
            };
            this.Repo.StatsCounterHits.Insert(hit);
            return hit;
        }

        public StatsCounterHit Hit(StatsCounterHitLink code)
        {
            var obj = code;
            var counter = this.GetCounter(obj.Id);
            ////if (counter.Name != obj.Name || obj.Category != counter.Category)
            ////    counter = this.GetCounter(obj.Category, obj.Name);

            var hit = new StatsCounterHit
            {
                CounterId = counter.Id,
                DateUtc = DateTime.UtcNow,
                NetworkId = obj.NetworkId,
                UserId = obj.UserId,
                Identifier = obj.Identifier,
            };
            this.Repo.StatsCounterHits.Insert(hit);
            return hit;
        }

        public IList<GetNewsletterStatsRow> GetNewsletterStats()
        {
            return this.Repo.StatsCounterHits.GetNewsletterStats(this.Services.NetworkId);
        }
        
    }
}
