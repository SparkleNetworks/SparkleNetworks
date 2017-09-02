using System;
using System.Text;

namespace vCalendar
{
    /// <summary>
    /// Write a vCalendar (ics)
    /// </summary>
    public class CalendarWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarWriter"/> class.
        /// </summary>
        public CalendarWriter()
        {
        }

        /// <summary>
        /// Gets the calendar as string.
        /// </summary>
        /// <param name="calendar">The calendar.</param>
        /// <returns></returns>
        public string GetCalendarAsString(Calendar calendar)
        {
            if (calendar == null)
                throw new ArgumentNullException("calendar");

            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VCALENDAR");

            if (string.IsNullOrEmpty(calendar.ProductId))
                calendar.SetProductId("MyCompany", "MyProduct", "V1.0", "EN");
            sb.AppendLine("PRODID:" + calendar.ProductId);

            if (!string.IsNullOrEmpty(calendar.Name))
                sb.AppendLine("X-WR-CALNAME:" + calendar.Name);

            sb.AppendLine("X-PUBLISHED-TTL:PT2H");
            sb.AppendLine("X-ORIGINAL-URL:" + calendar.OriginalUrl);
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("CALSCALE:" + calendar.Scale.ToString().ToUpperInvariant());
            sb.AppendLine("METHOD:" + calendar.Method.ToString().ToUpperInvariant());

            foreach (var item in calendar.Events)
            {
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTAMP:" + FormatDate(item.DateCreatedUtc));
                sb.AppendLine("LAST-MODIFIED:" + FormatDate(item.DateUpdatedUtc));
                sb.AppendLine("DTSTART:" + FormatDate(item.DateStartUtc));
                sb.AppendLine("DTEND:" + FormatDate(item.DateEndUtc));
                sb.AppendLine("UID:" + item.Uid);

                if (!string.IsNullOrEmpty(item.Summary))
                {
                    sb.Append("SUMMARY:");
                    FormatText(sb, item.Summary);
                }

                if (!string.IsNullOrEmpty(item.Location))
                {
                    sb.Append("LOCATION:");
                    FormatText(sb, item.Location);
                }

                if (item.Url != null)
                {
                    sb.Append("URL:");
                    FormatText(sb, item.Url.OriginalString);
                }

                if (!string.IsNullOrEmpty(item.Description))
                {
                    sb.Append("DESCRIPTION:");
                    FormatText(sb, item.Description);
                }
                
                sb.AppendLine("CLASS:" + item.Class.ToString().ToUpperInvariant());
                sb.AppendLine("STATUS:" + item.Status.ToString().ToUpperInvariant());
                sb.AppendLine("PARTSTAT:" + item.PartyStatus.ToString().ToUpperInvariant());
                sb.AppendLine("END:VEVENT"); 
            }

            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }

        /// <summary>
        /// Formats the text.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="value">The value.</param>
        private void FormatText(StringBuilder sb, string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (i != 0 && i % 36 == 0)
                {
                    sb.AppendLine();
                    sb.Append(' ');
                }

                switch (value[i])
                {
                    case '\r':
                    case '\n':
                        sb.Append("\\n");
                        break;
                    default:
                        sb.Append(value[i]);
                        break;
                }
            }
            sb.AppendLine();
        }

        /// <summary>
        /// Formats the date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        private string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyyMMdd\THHmmss\Z");
        }
    }
}
