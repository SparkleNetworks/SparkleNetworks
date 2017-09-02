
namespace Sparkle.Services.Networks.PartnerResources
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Tags;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CityPartnershipsModel
    {
        public CityTagModel City { get; set; }

        public int PartnersCount { get; set; }

        public IList<PartnerResourceEditRequest> Partners { get; set; }

        public IList<Tag2Model> Tags { get; set; }

        public bool IsAdmin { get; set; }
    }
}
