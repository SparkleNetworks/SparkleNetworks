
namespace Sparkle.WebBase
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using Sparkle.Infrastructure.Crypto;
    using Srk.BetaServices.ClientApi;

    /// <summary>
    /// Handles error reporting.
    /// </summary>
    public static class HttpErrorReport
    {
        public static Action<HttpContextBase, StringBuilder> BaseSessionDetails { get; set; }

        public static Action<HttpContext, StringBuilder> SessionDetails { get; set; }

        static HttpErrorReport()
        {
            // find configuration values
            var url = ConfigurationManager.AppSettings["CrashReport.Url"];
            var key = ConfigurationManager.AppSettings["CrashReport.ApiKey"];
            var isEnabled = ConfigurationManager.AppSettings["CrashReport.IsEnabled"];
            bool isEnabledValue;

            // if configured, then setup the BetaservicesClientFactory to be used.
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(key) && bool.TryParse(isEnabled, out isEnabledValue) && isEnabledValue)
            {
                BetaservicesClientFactory.Default = new BetaservicesClientFactory(key, "IIS/7.5 (SparkleNetworks)", url);
            }
        }

        public static void Initialize()
        {
            // calling this ensures the static constructor is called
        }

        public static void Do(HttpContextBase context, bool handled, DateTime appStartTime, Assembly applicationEntryAssembly)
        {
            Do(context, handled, context.AllErrors, appStartTime, applicationEntryAssembly, null);
        }

        public static void Do(HttpContextBase context, bool handled, Exception error, DateTime appStartTime, Assembly applicationEntryAssembly)
        {
            Do(context, handled, new Exception[] { error }, appStartTime, applicationEntryAssembly, null);
        }

        public static void Do(HttpContextBase context, bool handled, Exception error, DateTime appStartTime, Assembly applicationEntryAssembly, string comment)
        {
            Do(context, handled, new Exception[] { error }, appStartTime, applicationEntryAssembly, comment);
        }

        public static void Do(HttpContextBase context, bool handled, Exception[] errors, DateTime appStartTime, Assembly applicationEntryAssembly, string comment)
        {
            if (errors == null || errors.Length == 0)
                return;

            if (BetaservicesClientFactory.Default == null)
                return;

            var entryAssembly = applicationEntryAssembly ?? typeof(HttpErrorReport).Assembly;
            var entryAssemblyTitle = AssemblyTitleAttribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
            var entryAssemblyVersion = AssemblyFileVersionAttribute.GetCustomAttribute(entryAssembly, typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;
            var process = Process.GetCurrentProcess();
            var currentmem = process.PrivateMemorySize64;
            var maxmem = process.PeakWorkingSet64;

            foreach (var error in errors)
            {
                var report = new ErrorReport
                {
                    AssemblyName = entryAssemblyTitle != null ? entryAssemblyTitle.Title : "Sparkle.Web",
                    AssemblyVersion = entryAssemblyVersion != null ? entryAssemblyVersion.Version : "1.0.0.0",
                    OSPlatform = Environment.OSVersion.Platform.ToString(),
                    OSVersion = Environment.OSVersion.Version.ToString(),
                    AppStartTime = appStartTime,
                    AppErrorTime = DateTime.UtcNow,
                    Culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                    AppCurrentMemoryUsage = currentmem,
                    AppPeakMemoryUsage = maxmem,
                };
                report.DeploymentInstance = Environment.MachineName;

                if (context != null)
                {
                    AppendContextDetails(context, report);
                }

                if (error != null)
                    report.SetException(error);
                else
                    report.SetNonException("no exception");

                if (comment != null)
                    report.AppendComment(comment);

                try
                {
                    var client = BetaservicesClientFactory.Default.CreateDefaultClient();
                    report.DeploymentKind = "WebSite";
                    report.AppExitTime = DateTime.UtcNow;

                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        if (!string.IsNullOrEmpty(report.Comment))
                            report.Comment += Environment.NewLine;
                        report.Comment += "Debugger was attached during report. ";
                    }

                    int id = client.ReportCrash(report);
                    Trace.TraceInformation("Error report #" + id + " sent.");
                }
                catch (Exception ex)
                {
                    Trace.TraceError("HttpErrorReport.Do: error posting error report" + Environment.NewLine + ex.ToString());
                    if (ex.IsFatal())
                        throw;
                }
            }
        }

        public static void Do(HttpContext context, bool handled, DateTime appStartTime, Assembly applicationEntryAssembly)
        {
            Do(context, handled, context.AllErrors, appStartTime, applicationEntryAssembly, null);
        }

        public static void Do(HttpContext context, bool handled, Exception[] errors, DateTime appStartTime, Assembly applicationEntryAssembly, string comment)
        {
            if (errors == null || errors.Length == 0)
                return;

            if (BetaservicesClientFactory.Default == null)
                return;

            var entryAssembly = applicationEntryAssembly ?? typeof(HttpErrorReport).Assembly;
            var entryAssemblyTitle = AssemblyTitleAttribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
            var entryAssemblyVersion = AssemblyFileVersionAttribute.GetCustomAttribute(entryAssembly, typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;
            var process = Process.GetCurrentProcess();
            var currentmem = process.PrivateMemorySize64;
            var maxmem = process.PeakWorkingSet64;

            foreach (var error in errors)
            {
                var report = new ErrorReport
                {
                    AssemblyName = entryAssemblyTitle != null ? entryAssemblyTitle.Title : "Sparkle.Web",
                    AssemblyVersion = entryAssemblyVersion != null ? entryAssemblyVersion.Version : "1.0.0.0",
                    OSPlatform = Environment.OSVersion.Platform.ToString(),
                    OSVersion = Environment.OSVersion.Version.ToString(),
                    AppStartTime = appStartTime,
                    AppErrorTime = DateTime.UtcNow,
                    Culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                    AppCurrentMemoryUsage = currentmem,
                    AppPeakMemoryUsage = maxmem,
                };
                report.DeploymentInstance = Environment.MachineName;

                if (context != null)
                {
                    AppendContextDetails(context, report);
                }

                if (error != null)
                    report.SetException(error);
                else
                    report.SetNonException("no exception");

                if (comment != null)
                    report.AppendComment(comment);

                try
                {

                    var client = BetaservicesClientFactory.Default.CreateDefaultClient();
                    report.DeploymentKind = "WebSite";
                    report.AppExitTime = DateTime.UtcNow;
                    report.DeploymentInstance = Environment.MachineName;

                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        if (!string.IsNullOrEmpty(report.Comment))
                            report.Comment += Environment.NewLine;
                        report.Comment += "Debugger was attached during report. ";
                    }

                    int id = client.ReportCrash(report);
                    Trace.TraceInformation("Error report #" + id + " sent.");
                }
                catch (Exception ex)
                {
                    Trace.TraceError("HttpErrorReport.Do: error posting error report" + Environment.NewLine + ex.ToString());
                    if (ex.IsFatal())
                        throw;
                }
            }
        }

        private static void AppendExceptions(bool handled, Exception[] errors, StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine("Exceptions:");
            sb.AppendLine();
            int i = 0;
            foreach (var err in errors)
            {
                sb.AppendLine("ERROR #" + i++ + " was " + (handled ? "handled" : "unhandled"));
                Exception ex = err;
                int j = 0;
                while (ex != null)
                {
                    sb.Append(j++ == 0 ? "root: " : ("inner " + j + ": "));
                    sb.AppendLine(ex.ToString());
                    ex = ex.InnerException;
                }
                sb.AppendLine("----");
                sb.AppendLine();
            }
            sb.AppendLine("eod");
            sb.AppendLine();
        }

        private static void AppendContextDetails(HttpContextBase context, ErrorReport report)
        {   
            report.HttpHost = context.Request.Url.Host;
            report.HttpMethod = context.Request.HttpMethod;
            report.HttpRequest = context.Request.Url != null ? context.Request.Url.ToString() : null;
            report.HttpReferer = context.Request.UrlReferrer != null ? context.Request.UrlReferrer.ToString() : null;
            

            if (context.User != null && context.User.Identity != null)
                report.UserId = context.User.Identity.Name;

            var comment = new StringBuilder();
            comment.AppendLine("Browser: " + context.Request.UserAgent ?? "-");
            comment.AppendLine("HTTP response code: " + context.Response.StatusCode);
         
            comment.Append("User: ");
            if (context.Session != null)
            {
                if (BaseSessionDetails != null)
                    BaseSessionDetails(context, comment);
            }
            else if (context.User != null && context.User.Identity != null && context.User.Identity.Name != null)
            {
                comment.AppendLine(context.User.Identity.Name);
            }
            else
            {
                comment.AppendLine("-");
            }

            if (context.Request != null & context.Request.Headers["X-Requested-With"] != null)
            {
                comment.AppendLine("X-Requested-With: " + context.Request.Headers["X-Requested-With"]);
            }

            if (context.Request != null & context.Request.UserHostAddress != null)
            {
                comment.AppendLine("User host: " + context.Request.UserHostAddress + " (" + context.Request.UserHostName + ")");
                comment.AppendLine("           http://whois.domaintools.com/" + Uri.EscapeDataString(context.Request.UserHostAddress));
            }

            if (context.Request != null & context.Request.Form != null)
            {
                comment.Append("POST form data: ");
                string sep = string.Empty;
                foreach (var key in context.Request.Form.AllKeys)
                {
                    if (key.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0)
                        continue; // exclude this field from error reporting

                    comment.Append(sep + key + "=" + context.Request.Form[key]);
                    sep = "&";
                }

                comment.AppendLine();
            }

            report.Comment = comment.ToString();
        }

        private static void AppendContextDetails(HttpContext context, ErrorReport report)
        {
            report.HttpHost = context.Request.Url.Host;
            report.HttpMethod = context.Request.HttpMethod;
            report.HttpRequest = context.Request.Url != null ? context.Request.Url.ToString() : null;
            report.HttpReferer = context.Request.UrlReferrer != null ? context.Request.UrlReferrer.ToString() : null;


            if (context.User != null && context.User.Identity != null)
                report.UserId = context.User.Identity.Name;

            var comment = new StringBuilder();
            comment.AppendLine("Browser: " + context.Request.UserAgent ?? "-");
            comment.AppendLine("HTTP response code: " + context.Response.StatusCode);

            comment.Append("User: ");
            if (context.Session != null)
            {
                if (SessionDetails != null)
                    SessionDetails(context, comment);
            }
            else if (context.User != null && context.User.Identity != null && context.User.Identity.Name != null)
            {
                comment.AppendLine(context.User.Identity.Name);
            }
            else
            {
                comment.AppendLine("-");
            }

            if (context.Request != null & context.Request.Headers["X-Requested-With"] != null)
            {
                comment.AppendLine("X-Requested-With: " + context.Request.Headers["X-Requested-With"]);
            }

            if (context.Request != null & context.Request.UserHostAddress != null)
            {
                comment.AppendLine("User host: " + context.Request.UserHostAddress + " (" + context.Request.UserHostName + ")");
                comment.AppendLine("           http://whois.domaintools.com/" + Uri.EscapeDataString(context.Request.UserHostAddress));
            }

            if (context.Request != null & context.Request.Form != null)
            {
                comment.Append("POST form data: ");
                string sep = string.Empty;
                foreach (var key in context.Request.Form.AllKeys)
                {
                    if (key.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0)
                        continue; // exclude this field from error reporting

                    comment.Append(sep + key + "=" + context.Request.Form[key]);
                    sep = " & ";
                }

                comment.AppendLine();
            }

            report.Comment = comment.ToString();
        }
    }
}
