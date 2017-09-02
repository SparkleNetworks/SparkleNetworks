
namespace Sparkle.Data.Networks.Options
{
    [System.Flags]
    public enum EventOptions
    {
        None = 0x0000,
        Scope = 0x0002,
        Category = 0x0004,
        EventsMembers = 0x0010,
        EventsMembersPeople = 0x0020,
        Owner = 0x0040,
        Place = 0x0080,
    }
}
