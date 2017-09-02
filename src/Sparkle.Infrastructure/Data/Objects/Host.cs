
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
    /// An host is an environment (production/staging/development or server1/server2/client1).
    /// </summary>
    public class Host
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        public string Name { get; set; }
    }
}
