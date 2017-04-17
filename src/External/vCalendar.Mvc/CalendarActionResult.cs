using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace vCalendar.Mvc
{
    public class CalendarActionResult : ActionResult
    {
        public string ContentType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public Calendar Data { get; set; }
        public string FileName { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = this.ContentType ?? "text/calendar";
            if (FileName != null)
                response.AddHeader("Content-Disposition", "inline; filename=" + HttpUtility.UrlPathEncode(this.FileName) + "");

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                var writer = new CalendarWriter();
                response.Write(writer.GetCalendarAsString(this.Data));
            }
        }
    }
}
