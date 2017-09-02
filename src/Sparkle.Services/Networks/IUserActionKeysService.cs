
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IUserActionKeysService
    {
        UserActionKey GetLatestAction(int userId, string key);

        UserActionKey Update(UserActionKey action);

        UserActionKey GetById(int id);
    }
}
