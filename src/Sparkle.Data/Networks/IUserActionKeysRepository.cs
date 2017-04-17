
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository("UserActionKeys")]
    public interface IUserActionKeysRepository : IBaseNetworkRepository<UserActionKey, int>
    {
        UserActionKey GetLatestAction(int userID, string key);
    }
}
