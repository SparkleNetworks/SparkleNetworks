
namespace Sparkle.Services.Networks.Models.Timelines
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AddTimelineTagRequest : BaseRequest
    {
        public AddTimelineTagRequest()
            : base()
        {
    
        }

        public AddTimelineTagRequest(int userid, int timelineItemid, string tagname)
            : base()
        {
            UserId = userid;
            TimelineItemidId = timelineItemid;
            TagName = tagname;
        }

        public int UserId { get; set; }

        public int TimelineItemidId { get; set; }

        public string TagName { get; set; }

        public int TagId { get; set; }

        public WallItemDeleteReason Reason { get; set; }
    }
}
