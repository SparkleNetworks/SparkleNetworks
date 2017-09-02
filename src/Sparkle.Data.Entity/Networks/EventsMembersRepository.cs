
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Data.Objects;

    public class EventsMembersRepository : BaseNetworkRepository<EventMember>, IEventsMembersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public EventsMembersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.EventMembers)
        {
        }

        public IQueryable<EventMember> SelectRegistered(IList<string> options)
        {
            if (options.Count > 0)
            {
                return SelectWithOptions(this.Set, options);
            }

            return this.Set;
        }

        public IQueryable<User> GetGuestList(int eventId)
        {
            return this.Context.Users
                .Include("EventMembers")
                .Include("Company")
                .Include("Job")
                .Where(o => o.EventMembers.Any(n => n.EventId == eventId));       
        }

        public IQueryable<User> GetGuestList(int EventId, EventMemberState state)
        {
            switch (state)
            {
                case EventMemberState.HasAccepted:
                    return this.Context.Users.Include("EventMembers").Include("Company").Include("Job")
                        .Where(o => o.EventMembers.Any(n => n.EventId == EventId && n.State == (int)EventMemberState.HasAccepted));       
                case EventMemberState.IsInvited:
                    return this.Context.Users.Include("EventMembers").Include("Company").Include("Job")
                        .Where(o => o.EventMembers.Any(n => n.EventId == EventId && n.State == (int)EventMemberState.IsInvited));
                case EventMemberState.MaybeJoin:
                    return this.Context.Users.Include("EventMembers").Include("Company").Include("Job")
                        .Where(o => o.EventMembers.Any(n => n.EventId == EventId && n.State == (int)EventMemberState.MaybeJoin));       
                case EventMemberState.WontCome:
                    return this.Context.Users.Include("EventMembers").Include("Company").Include("Job")
                        .Where(o => o.EventMembers.Any(n => n.EventId == EventId && n.State == (int)EventMemberState.WontCome));
                case EventMemberState.WantJoin:
                    return this.Context.Users.Include("EventMembers").Include("Company").Include("Job")
                        .Where(o => o.EventMembers.Any(n => n.EventId == EventId && n.State == (int)EventMemberState.WantJoin));
                default:
                    return this.Context.Users.Include("EventMembers").Include("Company").Include("Job")
                        .Where(o => o.EventMembers.Any(n => n.EventId == EventId));       
            }
        }

        public IQueryable<User> GetRegisteredList(int EventId,bool Accepted)
        {
            // TODO: todo quoi ?
            return this.Context.Users.Include("EventMembers").Include("Company").Include("Job")
                .Where(o => o.EventMembers.Any(n => n.EventId == EventId));
        }
        /*
        [Obsolete("TRANSACTION UNSAFE")]
        public int InsertRegistered(EventMember item)
        {
            using (var dc = this.NewContext)
            {
                this.GetSet(dc).AddObject(item);
                dc.SaveChanges();
            }

            return item.EventId;
        }
        */
        public EventMember UpdateRegistered(EventMember item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("EventsMembersRepository.UpdateRegistered: This cannot be used within a transaction.");

            using (var model = this.GetNewContext())
            {
                var set = this.GetSet(model);
                model.ContextOptions.LazyLoadingEnabled = true;
                EntityKey key = model.CreateEntityKey(set.EntitySet.Name, item);
                object outItem;
                if (model.TryGetObjectByKey(key, out outItem))
                {
                    set.ApplyCurrentValues(item);
                    this.OnUpdateOverride(model, item);
                    model.SaveChanges();
                }
            }

            return item;
        }

        public void DeleteRegistered(EventMember item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("EventsMembersRepository.DeleteRegistered: This cannot be used within a transaction.");

            using (var DC = this.GetNewContext())
            {
                EntityKey key = DC.CreateEntityKey(DC.EventMembers.EntitySet.Name, item);
                Object OutItem;
                if (DC.TryGetObjectByKey(key, out OutItem))
                {
                    DC.DeleteObject(OutItem);
                    DC.SaveChanges();
                }
            }
        }

        public IList<EventMember> GetById(int[] userIds, int eventId)
        {
            return this.Set
                .Where(m => m.EventId == eventId && userIds.Contains(m.UserId))
                .ToList();
        }

        public EventMember GetById(int userId, int eventId)
        {
            return this.Set
                .Where(m => m.EventId == eventId && userId == m.UserId)
                .SingleOrDefault();
        }

        public IList<EventMember> GetByEventId(int id, EventMemberOptions options)
        {
            return this.NewQuery(options)
                .Where(m => m.EventId == id)
                .OrderBy(m => m.State)
                .ToList();
        }

        public IDictionary<int, IList<EventMember>> GetByEventId(int[] eventIds, EventMemberOptions options)
        {
            return this.GetByEventId(eventIds, options, false);
        }
    
        public IDictionary<int, IList<EventMember>> GetByEventId(int[] eventIds, EventMemberOptions options, bool comingOnly)
        {
            var accepted = (int)EventMemberState.HasAccepted;
            var maybe = (int)EventMemberState.MaybeJoin;

            return this.NewQuery(options)
                .Where(m => eventIds.Contains(m.EventId)
                    && (!comingOnly || (m.State == accepted || m.State == maybe)))
                .OrderBy(m => m.State)
                .ToList()
                .GroupBy(m => m.EventId)
                .ToDictionary(g => g.Key, g => (IList<EventMember>)g.ToList());
        }
    
        public IQueryable<EventMember> NewQuery(EventMemberOptions options)
        {
            ObjectQuery<EventMember> query = this.Set;
            
            if ((options & EventMemberOptions.Event) == EventMemberOptions.Event)
                query = query.Include("Event");

            if ((options & EventMemberOptions.User) == EventMemberOptions.User)
                query = query.Include("User");

            if ((options & EventMemberOptions.CompanyUser) == EventMemberOptions.CompanyUser)
                query = query.Include("User.Company");

            if ((options & EventMemberOptions.ApplyingUserSubscriptions) == EventMemberOptions.ApplyingUserSubscriptions)
                query = query.Include("User.ApplyingSubscriptions");

            return query;
        }

        public EventMember GetMembership(int eventId, int userId)
        {
            return this.NewQuery(EventMemberOptions.None)
                .Where(x => x.EventId == eventId && x.UserId == userId)
                .SingleOrDefault();
        }

        public int CountAll(int networkId)
        {
            return this.Set
                .Where(x => x.User.NetworkId == networkId)
                .Count();
        }

        public int CountActualMembers(int networkId)
        {
            var states = new int[]
            {
                (int)EventMemberState.HasAccepted,
                (int)EventMemberState.MaybeJoin,
            };
            return this.Set
                .Where(x => x.User.NetworkId == networkId && states.Contains(x.State))
                .Count();
        }

        public int CountInvitedMembers(int networkId)
        {
            var state = (int)EventMemberState.IsInvited;
            return this.Set
                .Where(x => x.User.NetworkId == networkId && state.Equals(x.State))
                .Count();
        }

        protected virtual void OnUpdateOverride(NetworksEntities model, EventMember itemToUpdate)
        {
            itemToUpdate.DateUpdatedUtc = DateTime.UtcNow;
            if (itemToUpdate is INetworkEntity)
            {
                this.VerifyNetworkId((INetworkEntity)itemToUpdate);
            }
        }

        protected override void OnInsertOverride(NetworksEntities model, EventMember itemToInsert)
        {
            itemToInsert.DateCreatedUtc = DateTime.UtcNow;

            base.OnInsertOverride(model, itemToInsert);
        }
    }
}
