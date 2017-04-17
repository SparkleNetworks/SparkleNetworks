
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class AchievementsUsersRepository : BaseNetworkRepository<AchievementsUser>, IAchievementsUsersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public AchievementsUsersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.AchievementsUsers)
        {
        }

        public AchievementsUser GetById(int achievementId, int userId)
        {
            return this.Set.SingleOrDefault(a => a.UserId == userId && a.AchievementId == achievementId);
        }

        public AchievementsUser Update(AchievementsUser item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("AchievementsUsersRepository.Update: This cannot be used within a transaction.");

            using (var model = this.GetNewContext())
            {
                var set = this.GetSet(model);
                model.ContextOptions.LazyLoadingEnabled = true;
                EntityKey key = model.CreateEntityKey(set.EntitySet.Name, item);
                object outItem;
                if (model.TryGetObjectByKey(key, out outItem))
                {
                    set.ApplyCurrentValues(item);
                    model.SaveChanges();
                }
            }

            return item;
        }

        public void Delete(AchievementsUser item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("AchievementsUsersRepository.Delete: This cannot be used within a transaction.");

            using (var model = this.GetNewContext())
            {
                var set = this.GetSet(model);
                model.ContextOptions.LazyLoadingEnabled = true;
                EntityKey key = model.CreateEntityKey(set.EntitySet.Name, item);
                Object outItem;
                if (model.TryGetObjectByKey(key, out outItem))
                {
                    AchievementsUser itemInDc = (AchievementsUser)outItem;
                    model.DeleteObject(itemInDc);
                    model.SaveChanges();
                }
            }
        }
    }
}
