
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TimelineModel
    {
        public TimelineModel()
        {
            this.Url = "/Ajax/TimelineList";
            this.AutoLoad = true;
        }

        public void CheckIfCanPost(User me)
        {
            switch (TimelineMode)
            {
                case TimelineDisplayContext.Public:
                    this.CanPost = true;
                    break;
                case TimelineDisplayContext.Event:
                    this.CanPost = true;
                    break;
                case TimelineDisplayContext.Company:
                    if (this.TimelineMode == TimelineDisplayContext.Company)
                    {
                        if (me.CompanyID == this.TimelineId)
                            this.CanPost = true;
                    }
                    break;
                case TimelineDisplayContext.Project:
                    this.CanPost = true;
                    break;
                case TimelineDisplayContext.Team:
                    this.CanPost = true;
                    break;
                case TimelineDisplayContext.Profile:
                    this.CanPost = false;
                    break;
            }
        }

        public override string ToString()
        {
            return "TimelineModel '" + this.TimelineName + "' " + (this.CanPost ? "RW" : "RO") + " id:" + this.TimelineId;
        }

        public string Url { get; set; }

        public bool AutoLoad { get; set; }

        public TimelineDisplayContext TimelineMode { get; set; }

        public bool CanPost { get; set; }

        public int TimelineId { get; set; }

        public string TimelineName { get; set; }

        public string DefaultMessage { get; set; }

        public string PersonPictureUrl { get; set; }
    }
}
