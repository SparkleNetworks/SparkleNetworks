
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IAlbumsRepository : IBaseNetworkRepository<Album, int>
    {
    }

    [Repository("Pictures")]
    public interface IPicturesRepository : IBaseNetworkRepository<Picture, int>
    {
    }
}
