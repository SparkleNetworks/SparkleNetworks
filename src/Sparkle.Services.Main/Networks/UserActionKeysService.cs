
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class UserActionKeysService : ServiceBase, IUserActionKeysService
    {
        [DebuggerStepThrough]
        internal UserActionKeysService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public UserActionKey GetLatestAction(int userId, string key)
        {
            return this.Repo.UserActionKeys.GetLatestAction(userId, key);
        }

        public UserActionKey Update(UserActionKey action)
        {
            return this.Repo.UserActionKeys.Update(action);
        }

        public UserActionKey GetById(int id)
        {
            return this.Repo.UserActionKeys.GetById(id);
        }
    }
}
