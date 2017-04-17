
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Objects;
    using Neutral = Sparkle.Entities.Networks.Neutral;

    public interface IStatsCountersService
    {
        void SetupWeeklyNewsletterCounters();
        void SetupDailyNewsletterCounters();
        void SetupAffiliateCounters();
        void SetupPartnerResourceCounters();

        Neutral.StatsCounter GetCounter(int id);
        Neutral.StatsCounter GetCounter(string category, string name);
        Neutral.StatsCounter CreateCounter(string category, string name);

        StatsCounterHitLink GetTrackingCode(string category, string name, string identifier = null, int? userId = null, int? networkId = null);
        StatsCounterHit Hit(string category, string name, string identifier = null, int? userId = null, int? networkId = null);
        StatsCounterHit Hit(string trackingCode);
        StatsCounterHit Hit(StatsCounterHitLink code);

        IList<GetNewsletterStatsRow> GetNewsletterStats();
    }
}
