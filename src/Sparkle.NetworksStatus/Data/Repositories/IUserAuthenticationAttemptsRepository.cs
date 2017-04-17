
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    partial interface IUserAuthenticationAttemptsRepository
    {
        IList<UserAuthenticationAttempt> GetByUserId(int userId);
    }
}
