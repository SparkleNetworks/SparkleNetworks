
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Linq;
    using System.Collections.Generic;

    public class EventPublicMembersRepository : BaseNetworkRepositoryInt<EventPublicMember>, IEventPublicMembersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public EventPublicMembersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.EventPublicMembers)
        {
        }

        public EventPublicMember GetByEmail(string emailAddress)
        {
            return this.Set.Where(m => m.Email.ToLower() == emailAddress.ToLower()).SingleOrDefault();
        }

        public IList<EventPublicMember> GetByEventId(int eventId)
        {
            return this.Set
                .Where(m => m.EventId == eventId)
                .ToList();
        }

        public IDictionary<int, IList<EventPublicMember>> GetByEventId(int[] eventIds)
        {
            return this.Set
                .Where(m => eventIds.Contains(m.EventId))
                .OrderBy(m => m.FirstName)
                .OrderBy(m => m.EventId)
                .ToList()
                .GroupBy(e => e.EventId)
                .ToDictionary(g => g.Key, g => (IList<EventPublicMember>)g.ToList());
        }

        protected override void OnUpdateOverride(NetworksEntities model, EventPublicMember itemToUpdate)
        {
            itemToUpdate.DateUpdatedUtc = DateTime.UtcNow;

            base.OnUpdateOverride(model, itemToUpdate);
        }

        protected override void OnInsertOverride(NetworksEntities model, EventPublicMember itemToInsert)
        {
            itemToInsert.DateCreatedUtc = DateTime.UtcNow;

            base.OnInsertOverride(model, itemToInsert);
        }

        public IList<EventPublicMember> GetAll()
        {
            return this.Set
                .ToList();
        }

        public int CountComing(int networkId)
        {
            return this.Set
                .Where(o => o.State == (int)EventMemberState.HasAccepted && o.Event.NetworkId == networkId)
                .Count();
        }
    }
}
