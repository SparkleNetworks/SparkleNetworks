
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure.Data.Objects
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A universe can be seen as an instance, a client, a tenant...
    /// </summary>
    public class Universe
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public short Status { get; set; }
    }
}
