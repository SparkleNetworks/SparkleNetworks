
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using System.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Data.Objects;
    using Sparkle.Data.Options;

    public class FriendsRepository : BaseNetworkRepository<Contact>, IFriendsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public FriendsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Contacts)
        {
        }

        public IQueryable<User> GetContactsByUserId(int userId)
        {
            return this.Context.Users
                .Include("Contacts").Include("ContactsOf").Include("Job").Include("Company")
                .Where(o =>
                            o.Contacts.Any(n => n.ContactId == userId)
                            ||
                            o.ContactsOf.Any(i => i.UserId == userId)
                       );
        }

        public IQueryable<UsersView> AltGetContactsByUserId(int userId)
        {
            var contacts = this.Context.Contacts
                .Where(o => o.UserId == userId || o.ContactId == userId)
                .Where(o => o.UserId != o.ContactId)
                .ToList();

            int[] ids = new int[contacts.Count];
            int it = 0;
            foreach (var item in contacts)
            {
                if (item.UserId != userId)
                    ids[it++] = item.UserId;
                else if (item.ContactId != userId)
                    ids[it++] = item.ContactId;
            }

            return this.Context.UsersViews
                .Where(o => ids.Contains(o.Id));
        }

        public Contact SelectFriendByUserIdAndFriendId(int userId, int friendId)
        {
            return this.Context.Contacts.SingleOrDefault(o => o.UserId == userId && o.ContactId == friendId);
        }

        public void DeleteFriends(Contact item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("FriendsRepository.DeleteFriends: This cannot be used within a transaction.");
            
            using (var dc = this.GetNewContext())
            {
                EntityKey key = dc.CreateEntityKey(dc.Contacts.EntitySet.Name, item);
                Object outItem;
                if (dc.TryGetObjectByKey(key, out outItem))
                {
                    dc.DeleteObject(outItem);
                    dc.SaveChanges();
                }
            }
        }

        public Contact InsertFriends(Contact item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("FriendsRepository.InsertFriends: This cannot be used within a transaction.");
            
            using (var dc = this.GetNewContext())
            {
                dc.AddToContacts(item);
                dc.SaveChanges();
            }
            return item;
        }

        public Contact UpdateFriends(Contact item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("FriendsRepository.UpdateFriends: This cannot be used within a transaction.");
            
            using (var dc = this.GetNewContext())
            {
                EntityKey key = dc.CreateEntityKey(dc.Contacts.EntitySet.Name, item);
                Object outItem;
                if (dc.TryGetObjectByKey(key, out outItem))
                {
                    dc.Contacts.ApplyCurrentValues(item);
                    dc.SaveChanges();
                }
            }
            return item;
        }


        public int[] GetUsersContactIds(int userId)
        {
            return this.Context.GetUsersContactIds(userId).Select(o => o.Value).ToArray();
        }
    }
}