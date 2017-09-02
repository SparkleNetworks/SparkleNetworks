
namespace Sparkle.Data.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IMembershipRepository
    {
        IQueryable<AspnetMembership> Select();

        void LockMemberhipAccount(string applicationName, string username, DateTime dateTime);
    }
}
