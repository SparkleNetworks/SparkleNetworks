
namespace Sparkle.Entities.Networks
{
    /// <summary>
    /// An entity that must be associated with a network.
    /// </summary>
    public interface INetworkEntity
    {
        /// <summary>
        /// Gets or sets the network ID.
        /// </summary>
        int NetworkId { get; set; }

        /// <summary>
        /// Gets or sets the network.
        /// </summary>
        Network Network { get; }
    }
}
