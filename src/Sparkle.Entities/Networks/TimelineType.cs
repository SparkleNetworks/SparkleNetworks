
namespace Sparkle.Entities.Networks
{
    /// <summary>
    /// Type of timeline.
    /// Not to be confused with <see cref="TimelineItemType"/>.
    /// Company, Event, Group, Place...
    /// </summary>
    public enum TimelineType : byte
    {
        Public = 0,
        Private = 1,
        Profile = 2,
        Company = 3,
        CompanyNetwork = 4,
        Event = 5,
        Group = 6,
        Place = 7,
        Ad = 8,
        Project = 9,
        Team = 10,
    }
}
