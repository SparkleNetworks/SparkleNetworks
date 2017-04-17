
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SocialNetworkStateModel
    {
        public int NetworkId { get; set; }
        public SocialNetworkConnectionType Type { get; set; }
        public SocialNetworkState Entity { get; set; }
    }
}
