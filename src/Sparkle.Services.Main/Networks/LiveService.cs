
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Diagnostics;
    using SrkToolkit.Common;

    // 
    // This service is planned to be removed.
    // It has been replaced with UserPresence.
    // TODO LIST:
    // - [x] UserPresence should store presence days and times
    // - [x] Integrate UserPresence in Sidebar/Live
    // - [ ] Migrate Live table data to UserPresence table
    // - [ ] Migrate calls to this service to query UserPresences instead of Live
    // - [ ] Destroy Live table
    // 

    public class LiveService : ServiceBase, ILiveService
    {
        [DebuggerStepThrough]
        internal LiveService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ILiveRepository liveRepository
        {
            get { return this.Repo.Live; }
        }

        public Live GetById(int userId)
        {
            return liveRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.DateTime)
                .FirstOrDefault();
        }

        public bool IsOnline(int userId)
        {
            var live = liveRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.DateTime)
                .FirstOrDefault();

            if (live == null)
                return false;

            var now = DateTime.UtcNow;
            TimeSpan ts = now - live.DateTime;

            if (now != live.DateTime.Date)
                return false;

            double diff = ts.TotalMinutes;
            return diff < 11;
        }

        public IList<Live> GetOnline()
        {
            var now = DateTime.UtcNow;
            DateTime valideDate = now.AddMinutes(-5D);

            return liveRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(o => o.DateTime > valideDate)
                .OrderByDescending(o => o.DateTime)
                .ToList();
        }

        public void Log(byte status, int userId)
        {
            Live live = this.Services.Live.GetById(userId);
            var now = DateTime.UtcNow;
            if (live == null /*|| !live.DateTime.IsEqualTo(now, DateTimePrecision.Day)*/)
            {
                var item = new Live
                {
                    DateTime = now,
                    Status = status,
                    UserId = userId,
                    NetworkId = this.Services.NetworkId,
                };
                this.liveRepository.Insert(item);
            }
            else
            {
                this.VerifyNetwork(live);

                live.Status = status;
                live.DateTime = now;

                this.liveRepository.Update(live);
            }
        }

        public int GetOnlineCount()
        {
            var now = DateTime.UtcNow;
            DateTime valideDate = now.AddMinutes(-5D);

            return liveRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(o => o.DateTime > valideDate)
                .Count();
        }

        public DateTime? GetLastActivityDateByPersonId(int profileId)
        {
            Live live = liveRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(o => o.UserId == profileId)
                .OrderByDescending(o => o.DateTime)
                .FirstOrDefault();

            if (live != null)
                return live.DateTime;

            return null;
        }

        public int GetTodaysCount()
        {
            var now = DateTime.UtcNow;
            DateTime from = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Local);
            DateTime to = from.AddDays(1D);

            return liveRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(o => from <= o.DateTime && o.DateTime < to)
                .Count();
        }

        public Dictionary<int, DateTime> GetAllLastActivityDate()
        {
            return this.Repo.Live.Select()
                .Where(x => x.User.NetworkId == this.Services.NetworkId)
                .ToList()
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastActivity = g.Max(x => x.DateTime).AsLocal().ToUniversalTime(),
                })
                .ToDictionary(g => g.UserId, g => g.LastActivity);
        }

        public int GetUsersDaysCount(int userId)
        {
            var value = this.Repo.Live.GetUsersDaysCount(userId);
            return value;
        }
    }
}
