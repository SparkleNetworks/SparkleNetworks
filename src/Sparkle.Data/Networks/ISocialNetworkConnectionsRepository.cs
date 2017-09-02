
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ISocialNetworkConnectionsRepository : IBaseNetworkRepository<SocialNetworkConnection, int>
    {
        SocialNetworkConnection GetByUserIdAndType(int userId, SocialNetworkConnectionType type);

        int CountByUsernameAndType(string username, SocialNetworkConnectionType type);
    }
}
