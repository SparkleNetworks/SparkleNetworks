
namespace Sparkle.Services.Main.Internal
{
    using Sparkle.Services.Networks;
    using Srk.BetaServices.ClientApi;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    internal static class ServiceFactoryExtensions
    {
        internal static void ReportError(this IServiceFactory services, Exception ex, string remark)
        {
            var report = new ErrorReport
            {
                AssemblyName = "Sparkle.Web",
                OSPlatform = Environment.OSVersion.Platform.ToString(),
                OSVersion = Environment.OSVersion.Version.ToString(),
                AppErrorTime = DateTime.UtcNow,
                Culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
            };

            if (ex != null)
                report.SetException(ex);
            else
                report.SetNonException("no exception");

            if (remark != null)
                report.AppendComment(remark);

            var sb = new StringBuilder();
            sb.AppendLine("HostName: " + services.Application.HostName);
            sb.AppendLine("MainStatus: " + services.Application.MainStatus.ToString());
            sb.AppendLine("ProductName: " + services.Application.ProductName);
            sb.AppendLine("UniverseName: " + services.Application.UniverseName);

            report.AppendComment(sb.ToString());

            try
            {
                var defaultFactory = BetaservicesClientFactory.Default;
                if (defaultFactory == null)
                {
                    return;
                }

                var client = defaultFactory.CreateDefaultClient();
                report.DeploymentKind = "WebSite";
                report.AppExitTime = DateTime.UtcNow;
                report.DeploymentInstance = Environment.MachineName;

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (!string.IsNullOrEmpty(report.Comment))
                        report.Comment += Environment.NewLine;
                    report.Comment += "Debugger was attached during report. ";
                }

                client.ReportCrash(report);
            }
            catch (Exception exe)
            {
                Trace.TraceError("HttpErrorReport.Do: error posting error report" + Environment.NewLine + exe.ToString());
            }
        }
    }
}
