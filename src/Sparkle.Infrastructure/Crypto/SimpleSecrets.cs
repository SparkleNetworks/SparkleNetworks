
#if !SSC
namespace Sparkle.Infrastructure.Crypto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class SimpleSecrets
    {
        public static string NewLongRandom
        {
            // TODO: [OSP-Security] SimpleSecrets.NewLongRandom: do not use Guid, use RNG crypto service
            get { return (Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()).Replace("-", ""); }
        }

        public static string NewMediumRandom
        {
            // TODO: [OSP-Security] SimpleSecrets.NewMediumRandom: do not use Guid, use RNG crypto service
            get { return (Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()).Replace("-", ""); }
        }

        public static string NewShortRandom
        {
            // TODO: [OSP-Security] SimpleSecrets.NewShortRandom: do not use Guid, use RNG crypto service
            get { return Guid.NewGuid().ToString().Replace("-", ""); }
        }
    }
}
#endif
