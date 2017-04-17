
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    partial interface IUsersRepository
    {
        IList<User> GetAllSortedById(int id, int pageSize);

        int Count();

        User GetByPrimaryEmailId(int primaryEmailId);
    }
}
