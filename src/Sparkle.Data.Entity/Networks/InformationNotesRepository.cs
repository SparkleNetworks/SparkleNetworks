
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public class InformationNotesRepository : BaseNetworkRepositoryInt<InformationNote>, IInformationNotesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public InformationNotesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.InformationNotes)
        {
        }

        public IList<InformationNote> GetList(int offset, int count, InformationNotesFilter filter, InformationNoteOptions options)
        {
            var query = GetListQuery(filter);
            var items = query.ToList();
            return items;
        }

        private IQueryable<InformationNote> GetListQuery(InformationNotesFilter filter)
        {
            var query = this.Set.AsQueryable();

            if (filter != null)
            {
                if (filter.Filter == InformationNotesKnownFilter.All)
                {
                }
                else if (filter.Filter == InformationNotesKnownFilter.Active)
                {
                    var now = DateTime.UtcNow;
                    query = query.Where(x => x.StartDateUtc <= now && now <= x.EndDateUtc);
                }
            }
            return query;
        }

        public int CountList(InformationNotesFilter filter)
        {
            var query = GetListQuery(filter);
            var items = query.Count();
            return items;
        }

        public int Count()
        {
            var query = this.Set;
            var items = query.Count();
            return items;
        }
    }
}
