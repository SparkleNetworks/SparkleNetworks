
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class UserSettingsRepository : BaseNetworkRepository<UserSetting>, IUserSettingsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public UserSettingsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UserSettings)
        {
        }

        public UserSetting Update(UserSetting item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("UserSettingsRepository.Update: This cannot be used within a transaction.");

            using (var context = this.GetNewContext())
            {
                EntityKey key = context.CreateEntityKey(context.UserSettings.EntitySet.Name, item);
                Object OutItem;
                if (context.TryGetObjectByKey(key, out OutItem))
                {
                    context.UserSettings.ApplyCurrentValues(item);
                    context.SaveChanges();
                }
            }
            return item;
        }
    }
}
