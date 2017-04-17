
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure.Contracts
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Constants for service namespaces.
    /// </summary>
    public static class Names
    {
        /// <summary>
        /// The namespace for WCF services.
        /// </summary>
        public const string ServiceContractNamespace = "http://sparklenetworks.net/Services/Infrastructure/1.0/";

        /// <summary>
        /// The name of the configuration service.
        /// </summary>
        public const string Configuration = "Configuration";
    }
}
