
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;
    using System.Data.Objects.SqlClient;
    using System.Linq;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;

    public class LiveRepository : BaseNetworkRepository<Live, int>, ILiveRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public LiveRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Lives)
        {
        }

        public IList<LivePerson> GetOnline()
        {
            // TODO: enhance SQL query
            DateTime valideDate = DateTime.UtcNow.AddMinutes(-10D);

            IList<LivePerson> items = this.Set
                .Where(o => o.DateTime > valideDate)
                .OrderByDescending(o => o.DateTime)
                .Select(l => new LivePerson
                {
                    DateTime = l.DateTime,
                    FirstName = l.User.FirstName,
                    Id = l.Id,
                    LastName = l.User.LastName,
                    Status = l.Status,
                    UserId = l.UserId,
                })
                .ToList();

            ////foreach (var item in items)
            ////{
            ////    LivePerson query = this.Context.Users
            ////      .Where(o => o.Id == item.Id)
            ////      .Select(g => new LivePerson
            ////      {
            ////          FirstName = g.FirstName,
            ////          LastName =g.LastName,
            ////      })
            ////      .SingleOrDefault();

            ////    item.FirstName = query.FirstName;
            ////    item.LastName = query.LastName;
            ////}

            return items;
        }

        public int GetUsersDaysCount(int userId)
        {
            var value = this.Set
                .Where(x => x.UserId == userId)
                .Count();
            return value;
        }

        protected override Live GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(Live item)
        {
            return item.Id;
        }
    }
}
