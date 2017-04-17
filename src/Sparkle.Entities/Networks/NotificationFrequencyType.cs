
namespace Sparkle.Entities.Networks
{
    public enum NotificationFrequencyType : byte
    {
        /// <summary>
        /// No notification
        /// </summary>
        None = 0,

        /// <summary>
        /// An email is sent immediately for all new item.
        /// </summary>
        Immediate = 1,

        /// <summary>
        /// An email is sent on every morning.
        /// </summary>
        Daily = 2,

        /// <summary>
        /// An email is sent on friday morning.
        /// </summary>
        Weekly = 3,

        /// <summary>
        /// An email is sent once a month.
        /// </summary>
        Monthly = 4,

        /// <summary>
        /// Special test value with the behavior of <see cref="Daily"/>.
        /// </summary>
        DailyTest = 5,

        /// <summary>
        /// Special test value with the behavior of <see cref="Weekly"/>.
        /// </summary>
        WeeklyTest = 6,

        /// <summary>
        /// Special test value with the behavior of <see cref="Monthly"/>.
        /// </summary>
        MonthlyTest = 7,
    }
}
