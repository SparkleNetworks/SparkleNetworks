
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Data.Objects;

    public class SeekFriendsRepository : BaseNetworkRepository<SeekFriend>, ISeekFriendsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SeekFriendsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.SeekFriends)
        {
        }

        private IQueryable<SeekFriend> NewQuery(SeekFriendOptions options)
        {
            ObjectQuery<SeekFriend> query = this.Set;

            if ((options & SeekFriendOptions.Seeker) == SeekFriendOptions.Seeker)
                query = query.Include("Seeker");

            if ((options & SeekFriendOptions.SeekerCompany) == SeekFriendOptions.SeekerCompany)
                query = query.Include("Seeker.Company");

            if ((options & SeekFriendOptions.SeekerJob) == SeekFriendOptions.SeekerJob)
                query = query.Include("Seeker.Job");

            if ((options & SeekFriendOptions.Target) == SeekFriendOptions.Target)
                query = query.Include("Target");

            if ((options & SeekFriendOptions.TargetCompany) == SeekFriendOptions.TargetCompany)
                query = query.Include("Target.Company");

            if ((options & SeekFriendOptions.TargetJob) == SeekFriendOptions.TargetJob)
                query = query.Include("Target.Job");

            return query;
        }

        public IQueryable<SeekFriend> SelectSeekFriendsBySeekerId(IList<string> options, int seekerId)
        {
            return this.Context.SeekFriends.Include("Target").Where(o => o.SeekerId == seekerId);
        }

        public IQueryable<SeekFriend> SelectSeekFriendsByTargetId(int targetId, SeekFriendOptions options)
        {
            return this.NewQuery(options)
                .Where(o => o.TargetId == targetId);
        }

        public SeekFriend SelectSeekFriendsByTargetIdAndSeekerId(int TargetId, int SeekerId)
        {
            return this.Context.SeekFriends.SingleOrDefault(o => o.TargetId == TargetId && o.SeekerId == SeekerId);
        }

        public new int Insert(SeekFriend item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("SeekFriendsRepository.Insert: This cannot be used within a transaction.");

            using (var dc = this.GetNewContext())
            {
                dc.AddToSeekFriends(item);
                dc.SaveChanges();
            }

            return item.SeekerId;
        }

        public void Delete(SeekFriend item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("SeekFriendsRepository.Delete: This cannot be used within a transaction.");

            using (var dc = this.GetNewContext())
            {
                EntityKey key = dc.CreateEntityKey(dc.SeekFriends.EntitySet.Name, item);
                Object outItem;
                if (dc.TryGetObjectByKey(key, out outItem))
                {
                    dc.DeleteObject(outItem);
                    dc.SaveChanges();
                }
            }
        }

        public SeekFriend Update(SeekFriend item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("SeekFriendsRepository.Update: This cannot be used within a transaction.");
            
            using (var dc = this.GetNewContext())
            {
                EntityKey key = dc.CreateEntityKey(dc.SeekFriends.EntitySet.Name, item);
                Object outItem;
                if (dc.TryGetObjectByKey(key, out outItem))
                {
                    dc.SeekFriends.ApplyCurrentValues(item);
                    dc.SaveChanges();
                }
            }
            return item;
        }

        public IQueryable<SeekFriend> QuerySeekFriendsByTargetId(int targetId, SeekFriendOptions options)
        {
            return this.CreateQuery(options).Where(s => s.TargetId == targetId);
        }

        public IQueryable<SeekFriend> QuerySeekFriendsBySeekerId(int seekerId, SeekFriendOptions options)
        {
            return this.CreateQuery(options).Where(s => s.TargetId == seekerId);
        }

        public IQueryable<SeekFriend> CreateQuery(SeekFriendOptions options)
        {
            var query = (ObjectQuery<SeekFriend>)this.Set;

            if ((options & SeekFriendOptions.Seeker) == SeekFriendOptions.Seeker)
            {
                query = query.Include("Seeker");
            }

            if ((options & SeekFriendOptions.SeekerCompany) == SeekFriendOptions.SeekerCompany)
            {
                query = query.Include("Seeker.Company");
            }

            if ((options & SeekFriendOptions.SeekerJob) == SeekFriendOptions.SeekerJob)
            {
                query = query.Include("Seeker.Job");
            }

            if ((options & SeekFriendOptions.Target) == SeekFriendOptions.Target)
            {
                query = query.Include("Target");
            }

            if ((options & SeekFriendOptions.TargetCompany) == SeekFriendOptions.TargetCompany)
            {
                query = query.Include("Target.Company");
            }

            if ((options & SeekFriendOptions.TargetJob) == SeekFriendOptions.TargetJob)
            {
                query = query.Include("Target.Job");
            }

            return query;
        }

        public IList<SeekFriend> GetPendingByTargetId(int userId, SeekFriendOptions options)
        {
            return this.CreateQuery(options)
                .Where(s => s.HasAccepted == null && s.TargetId == userId)
                .ToList();
        }
    }
}