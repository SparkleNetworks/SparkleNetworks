
namespace Sparkle.Entities.Networks
{
    public class EnumTool
    {
        public static NotificationFrequencyType? GetNotificationFrequencyType(byte value)
        {
            return (NotificationFrequencyType)value;
        }

        public static TimelineType? GetTimelineType(byte value)
        {
            switch (value)
            {
                case 0:
                    return TimelineType.Public;
                case 1:
                    return TimelineType.Private;
                case 2:
                    return TimelineType.Profile;
                case 3:
                    return TimelineType.Company;
                case 4:
                    return TimelineType.CompanyNetwork;
                case 5:
                    return TimelineType.Event;
                case 6:
                    return TimelineType.Group;
                case 7:
                    return TimelineType.Place;
                case 8:
                    return TimelineType.Ad;
                case 9:
                    return TimelineType.Project;
                case 10:
                    return TimelineType.Team;
                default:
                    return null;
            }
        }
    }
}
