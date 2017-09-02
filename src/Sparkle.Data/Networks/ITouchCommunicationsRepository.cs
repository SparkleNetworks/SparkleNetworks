
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ITouchCommunicationsRepository : IBaseNetworkRepository<TouchCommunication, int>
    {
    }
}
