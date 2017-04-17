
namespace Sparkle.Common
{
    using Sparkle.Common.Resources;
    using System;
    using System.Text;

    public enum DurationUnit : byte
    {
        Nanosecond = 1,
        Millisecond = 4,
        Second = 7,
        Minute = 8,
        Hour = 9,
        Day = 10,
        Week = 11,
        Month = 12,
        Year = 13,
    }

    public static class DurationUnitExtensions
    {
        public static string Display(this DurationUnit unit, int value)
        {
            var key = "DurationUnit_" + unit + (value == 0 || value == 1 ? string.Empty : "_Plural");
            return value + " " + MainStrings.ResourceManager.GetString(key, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        public static DateTime Add(this DateTime date, int value, DurationUnit unit)
        {
            switch (unit)
            {
                case DurationUnit.Nanosecond:
                    throw new ArgumentException("Unit " + unit + " is not supported in DateTime", "unit");

                case DurationUnit.Millisecond:
                    return date.AddMilliseconds(value);

                case DurationUnit.Second:
                    return date.AddSeconds(value);

                case DurationUnit.Minute:
                    return date.AddMinutes(value);

                case DurationUnit.Hour:
                    return date.AddHours(value);

                case DurationUnit.Day:
                    return date.AddDays(value);

                case DurationUnit.Week:
                    return date.AddDays(7 * value);

                case DurationUnit.Month:
                    return date.AddMonths(value);
                    
                case DurationUnit.Year:
                    return date.AddYears(value);

                default:
                    throw new ArgumentException("Unit " + unit + " is not supported", "unit");
            }
        }

        public static DateTime Sub(this DateTime date, int value, DurationUnit unit)
        {
            return date.Add(-value, unit);
        }
    }
}
