
namespace Sparkle.Services.Networks.Timelines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    //
    // See work item 504
    //
    /*
    public class TimelineItemsModel
    {
        public IList<TimelineItemModel> Items { get; set; }

        public void Fill(IServiceFactory services, Entities.Networks.TimelineItemsConveyor conveyor)
        {
            this.DisplayContext = conveyor.TimelineDisplayContext;
            this.Items = new List<TimelineItemModel>(conveyor.TimelineItems.Count);
            var types = new List<Sparkle.Entities.Networks.TimelineType>();

            foreach (var item in conveyor.TimelineItems)
            {
                var model = new TimelineItemModel();
                model.Fill(services, conveyor, this.DisplayContext, item);
            }
        }

        public Entities.Networks.TimelineDisplayContext DisplayContext { get; set; }
    }

    public class TimelineItemModel
    {
        public TimelineItemModel()
        {
            this.Header = new TimelineItemHeaderModel();
            this.Footer = new TimelineItemFooterModel();
            this.Relations = new List<TimelineItemRelationModel>();
        }

        public IList<TimelineItemModel> Children { get; set; }
        public TimelineItemHeaderModel Header { get; set; }
        public TimelineItemFooterModel Footer { get; set; }
        public TimelineItemRelationModel MainRelation { get; set; }
        public IList<TimelineItemRelationModel> Relations { get; set; }
        public Sparkle.Entities.Networks.TimelineType TimelineType { get; set; }
        public Sparkle.Entities.Networks.TimelineItemType TimelineItemType { get; set; }

        internal void Fill(
            IServiceFactory services,
            Entities.Networks.TimelineItemsConveyor conveyor,
            Entities.Networks.TimelineDisplayContext displayContext,
            Entities.Networks.TimelineItem item)
        {
            bool hasType = false;
            Entities.Networks.TimelineType type;
            if (services.Wall.TryGetTimelineType(item, out type))
            {
                hasType = true;
            }

            this.FillMainRelation(services, conveyor, displayContext, item, type);

        }

        private void FillMainRelation(
            IServiceFactory services,
            Entities.Networks.TimelineItemsConveyor conveyor,
            Entities.Networks.TimelineDisplayContext displayContext,
            Entities.Networks.TimelineItem item,
            Entities.Networks.TimelineType type)
        {
            switch (type)
            {
                case Sparkle.Entities.Networks.TimelineType.Public:
                    break;
                case Sparkle.Entities.Networks.TimelineType.Private:
                    break;
                case Sparkle.Entities.Networks.TimelineType.Profile:
                    break;
                case Sparkle.Entities.Networks.TimelineType.Company:
                    break;
                case Sparkle.Entities.Networks.TimelineType.CompanyNetwork:
                    break;
                case Sparkle.Entities.Networks.TimelineType.Event:
                    break;
                case Sparkle.Entities.Networks.TimelineType.Group:
                    ////var group = conveyor.RelatedGroups[item.GroupId];
                    ////this.MainRelation = TimelineItemRelationModel.Create(group);
                    break;
                case Sparkle.Entities.Networks.TimelineType.Place:
                    break;
                case Sparkle.Entities.Networks.TimelineType.Ad:
                    break;
                case Sparkle.Entities.Networks.TimelineType.Project:
                    break;
                case Sparkle.Entities.Networks.TimelineType.Team:
                    break;
                default:
                    break;
            }
        }
    }

    public class TimelineItemHeaderModel
    {

    }

    public class TimelineItemFooterModel
    {

    }

    public class TimelineItemRelationModel
    {
        
    }
    */
}
