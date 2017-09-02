
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial interface IUserPasswordsRepository
    {
        IList<UserPassword> GetUserId(int userId);
    }
}
