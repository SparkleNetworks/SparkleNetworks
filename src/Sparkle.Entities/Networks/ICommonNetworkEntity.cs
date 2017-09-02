
namespace Sparkle.Entities.Networks
{
    /// <summary>
    /// An entity that may be associated with a network.
    /// </summary>
    public interface ICommonNetworkEntity
    {
        /// <summary>
        /// Gets or sets the network ID.
        /// </summary>
        int? NetworkId { get; }

        /// <summary>
        /// Gets or sets the network.
        /// </summary>
        Network Network { get; }
    }
}
