// <copyright file="DateTimeExtensions.cs" company="Srk">
//     Copyright © SandRock 2011. All rights reserved.
// </copyright>
// <author>SandRock</author>

namespace Sparkle.Common
{
    using System;
    using System.Globalization;
    using Sparkle.Common.Resources;

    /// <summary>
    /// Extension methods for <see cref="DateTime"/>s.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns a <see cref="DateTime"/> as a string like "3 days ago" or "In 12 minutes".
        /// Comparison is done against DateTime.Now.
        /// </summary>
        /// <param name="time">The point in time.</param>
        /// <returns>a formated string</returns>
        public static string ToNiceDelay(this DateTime time)
        {
            return time.ToNiceDelay(DateTime.Now);
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> as a string like "3 days ago" or "In 12 minutes".
        /// </summary>
        /// <param name="time">The point in time.</param>
        /// <param name="now">The actual time.</param>
        /// <returns>a formated string</returns>
        public static string ToNiceDelay(this DateTime time, DateTime now)
        {
            var diff = now.Subtract(time); // >0 is past    <0 is future
            var culture = CultureInfo.CurrentCulture;

            // just now
            if (diff.TotalMinutes < 3D && diff.TotalMinutes > -3D)
                return MainStrings.NiceDateTime_JustNow;

            // minutes
            if (diff.TotalMinutes < 60D && diff.TotalMinutes > 0D)
                return string.Format(culture, MainStrings.NiceDateTime_MinutesAgo_, diff.TotalMinutes);
            if (diff.TotalMinutes > -60D && diff.TotalMinutes < 0D)
                return string.Format(culture, MainStrings.NiceDateTime_InMinutes_, diff.TotalMinutes * -1D);

            if (time.Date == now.Date || ((int)Math.Abs(diff.TotalDays) == 1 && diff.Hours > -4D && diff.Hours < 4D))
            {
                int hourDiff = diff.Hours;
                if (hourDiff > 4 || hourDiff < -4)
                {
                    if (time.Hour >= 0 && time.Hour < 6)
                        return (now.Hour >= 0 && now.Hour < 6) ? MainStrings.NiceDateTime_Tonight : MainStrings.NiceDateTime_LastNight;
                    if (time.Hour >= 6 && time.Hour < 11)
                        return MainStrings.NiceDateTime_Morning;
                    if (time.Hour >= 11 && time.Hour < 14)
                        return MainStrings.NiceDateTime_Midday;
                    if (time.Hour >= 14 && time.Hour < 19)
                        return MainStrings.NiceDateTime_Afternoon;
                    if (time.Hour >= 19 && time.Hour < 22)
                        return MainStrings.NiceDateTime_Evening;
                    if (time.Hour >= 22)
                        return MainStrings.NiceDateTime_Tonight;
                }

                if (diff.TotalHours >= 1D || diff.TotalHours < 2D)
                    return string.Format(culture, MainStrings.NiceDateTime_OneHourAgo, hourDiff);
                if (diff.TotalHours <= -1D || diff.TotalHours > -2D)
                    return string.Format(culture, MainStrings.NiceDateTime_InOneHour, hourDiff);
                if (diff.TotalHours < 24D && diff.TotalHours > 0D)
                    return string.Format(culture, MainStrings.NiceDateTime_HoursAgo_, hourDiff);
                if (diff.TotalHours > -24D && diff.TotalHours < 0D)
                    return string.Format(culture, MainStrings.NiceDateTime_InHours_, -hourDiff);
            }


            if (now.Year != time.Year && Math.Abs(diff.TotalDays) > 350D)
            {
                int yearDiff = now.Year - time.Year;
                if (yearDiff == 1)
                    return MainStrings.NiceDateTime_PreviousYear;
                if (yearDiff == -1)
                    return MainStrings.NiceDateTime_NextYear;

                if (diff.TotalDays > 0D)
                    return string.Format(culture, MainStrings.NiceDateTime_YearsAgo_, yearDiff);
                else
                    return string.Format(culture, MainStrings.NiceDateTime_InYears_, -yearDiff);
            }

            // here we know the year is the same

            if (now.Month != time.Month)
            {
                int monthDiff = (now.Year * 12 + now.Month) - (time.Year * 12 + time.Month);
                if (monthDiff == 1)
                    return MainStrings.NiceDateTime_PreviousMonth;
                if (monthDiff == -1)
                    return MainStrings.NiceDateTime_NextMonth;

                if (diff.TotalDays > 0D)
                    return string.Format(culture, MainStrings.NiceDateTime_MonthsAgo_, monthDiff);
                else
                    return string.Format(culture, MainStrings.NiceDateTime_InMonths_, -monthDiff);
            }

            // here we know the year and month are the same

            if (now.Day != time.Day)
            {
                if (diff.TotalDays < 0D)
                {
                    if (time.Date == now.AddDays(1D).Date)
                    {
                        return MainStrings.NiceDateTime_Tomorrow;
                    }
                    else
                    {
                        return string.Format(culture, MainStrings.NiceDateTime_InDays_, -diff.TotalDays);
                    }
                }
                else
                {
                    if (time.Date == now.AddDays(-1D).Date)
                    {
                        return MainStrings.NiceDateTime_Yesterday;
                    }
                    else
                    {
                        return string.Format(culture, MainStrings.NiceDateTime_DaysAgo_, diff.TotalDays);
                    }
                }
            }

            // here we know the year, month and day are the same

            // here we know the year, month, day and hour are the same

            if (diff.TotalMinutes < 3D && diff.TotalMinutes > -3D)
            {
                return MainStrings.NiceDateTime_JustNow;
            }

            if (now.Date != time.Date && diff.TotalDays <= 31D && diff.TotalDays >= -31D)
            {
                if (diff.TotalDays > 0D)
                    return MainStrings.NiceDateTime_PreviousMonth;
                else
                    return MainStrings.NiceDateTime_NextMonth;
            }

            if (now.Year == time.Year)
            {
                if (diff.TotalDays > 0D)
                    return string.Format(culture, MainStrings.NiceDateTime_MonthsAgo_, Math.Round(diff.TotalDays / 30D));
                else
                    return string.Format(culture, MainStrings.NiceDateTime_InMonths_, Math.Round(diff.TotalDays / 30D * -1D));
            }

            return null;
        }

        public static string ToUrlDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static bool IsSameDay(this DateTime date, DateTime compare)
        {
            return date.Year == compare.Year && date.Month == compare.Month && date.Day == compare.Day;
        }
    }
}
