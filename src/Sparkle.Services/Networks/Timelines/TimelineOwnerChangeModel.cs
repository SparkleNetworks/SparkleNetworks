
namespace Sparkle.Services.Networks.Timelines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TimelineOwnerChangeModel
    {
        public List<Models.UserModel> AvailableNewOwners { get; set; }

        public IList<Entities.Networks.TimelineItem> TimelineItems { get; set; }

        public IList<Entities.Networks.TimelineItemComment> TimelineComments { get; set; }

        public string UserIdentifier { get; set; }
    }
}
