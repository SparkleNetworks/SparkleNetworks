
namespace Sparkle.Models.Setup
{
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class SocialNetworksModel
    {
        public IList<SocialNetworkStateModel> Networks { get; set; }
    }
}