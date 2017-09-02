
namespace Sparkle.WebStatus.Controllers
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sparkle.Data.Entity.Networks;
    using Sparkle.EmailTemplates;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.WebBase;
    using Sparkle.WebStatus.App_Start;
    using Sparkle.WebStatus.ViewModels;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Domain;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// This controller handles requests from external services.
    /// </summary>
    public class HooksController : Controller
    {
        static readonly string BasePath = LocalConfig.EmailsInboundDirectory;
        const string MandrilPrefix = "mandrill_events=";

        private static readonly string[] autoReplySubjects = new string[]
        {
            "Message automatique",
            "Réponse automatique",
            "Out of office",
            "Absence du bureau",
            "Remise différée",
            "Non remis",
        };

        private List<string> logFiles = new List<string>();

        public bool IsTemporaryDataEnabled
        {
            get
            {
                bool configValue;
                return this.HttpContext.Application["IsTemporaryDataEnabled"] != null
                    ? (bool)this.HttpContext.Application["IsTemporaryDataEnabled"]
                    : ConfigurationManager.AppSettings["IsTemporaryDataEnabled"] != null && bool.TryParse(ConfigurationManager.AppSettings["IsTemporaryDataEnabled"], out configValue)
                    ? configValue
                    : false;
            }
        }

        [Authorize]
        public ActionResult Index()
        {
            // this page is mostly empty
            this.NavigationLine().Add("Hooks", this.Url.Action("Index"));
            this.ViewBag.IsAuthenticated = this.User != null && this.User.Identity != null && this.User.Identity.IsAuthenticated;
            return this.View();
        }

        [Authorize]
        public ActionResult TemporaryData(bool? ChangeStatus)
        {
            this.NavigationLine().Add("Hooks", this.Url.Action("Index"));
            this.NavigationLine().Add("TemporaryData", this.Url.Action("TemporaryData"));

            if (ChangeStatus != null)
            {
                this.HttpContext.Application["IsTemporaryDataEnabled"] = ChangeStatus.Value;
                return this.RedirectToAction("TemporaryData");
            }

            this.ViewBag.IsEnabled = this.IsTemporaryDataEnabled;
            this.ViewBag.HandleInboundEmails = LocalConfig.HandleInboundEmails;

            return this.View(this.HttpContext.Application["inbound"] ?? new List<HttpRequestModel>());
        }

        [Authorize]
        public ActionResult TemporaryDataFile(Guid id)
        {
            var list = (List<HttpRequestModel>)(this.HttpContext.Application["inbound"] ?? new List<HttpRequestModel>());
            var model = list.SingleOrDefault(x => x.Id == id);
            if (model == null)
                return this.HttpNotFound();

            return this.File(model.PostBytes, "application/octet-stream", id + ".bin");
        }

        [Authorize]
        public ActionResult ClearTemporaryData()
        {
            this.HttpContext.Application["inbound"] = null;
            return this.RedirectToAction("TemporaryData");
        }

        /// <summary>
        /// MANDRILL HOOK ACTION for INBOUND EMAILS and EVENT NOTIFICATION.
        /// </summary>
        public ActionResult MandrillInboundEmail()
        {
            this.NavigationLine().Add("Hooks", this.Url.Action("Index"));
            this.NavigationLine().Add("MandrillInboundEmail", this.Url.Action("MandrillInboundEmail"));

            return new EmptyResult();
        }

        /// <summary>
        /// Mandrill has a test page for hooks, you can put the url of this action in the mandrill form.
        /// </summary>
        [HttpPost]
        public ActionResult InboundTest(string id)
        {
            return this.Inbound(id, MandrillInboundAction.None);
        }

        /// <summary>
        /// MANDRILL HOOK ACTION for INBOUND EMAILS.
        /// </summary>
        [HttpPost]
        public ActionResult MandrillInboundEmail(string id)
        {
            return this.Inbound(id, MandrillInboundAction.Email);
        }

        /// <summary>
        /// MANDRILL HOOK ACTION for EVENT NOTIFICATION.
        /// </summary>
        [HttpPost]
        public ActionResult MandrillInboundEvent(string id)
        {
            return this.Inbound(id, MandrillInboundAction.Event);
        }

        public enum MandrillInboundAction
        {
            None,
            Email,
            Event,
        }

        /// <summary>
        /// Handles an inbound mandrill email message.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private ActionResult Inbound(string id, MandrillInboundAction action)
        {
            // TODO: enhance app selection/instantiation with a cache of SparkleNetworksApplication
            ////this.InitializeInbound();

            var isTemporaryDataEnabled = this.HttpContext.Application["IsTemporaryDataEnabled"] != null ? (bool)this.HttpContext.Application["IsTemporaryDataEnabled"] : false;

            var report = new InboundEmailReport();
            string networkName = null;

            // Getting the POST request
            var memoryStream = new MemoryStream(this.Request.ContentLength);
            this.Request.InputStream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(memoryStream, Encoding.UTF8);
            var contentAsText = reader.ReadToEnd();
            memoryStream.Seek(0, SeekOrigin.Begin);
            report.RawJson = contentAsText;

            // Add json request to temporary data for dev purposes
            var tempModel = AddToTemporaryData(contentAsText, memoryStream.ToArray());
            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                // this does not work
                ////var json = this.Request.Form["mandrill_events"];

                var json = contentAsText;

                // Deserialize json into InboundEmailModel
                var model = JsonToInboundModel(json, tempModel, action);
                if (model != null && model.Length > 0)
                {
                    tempModel.Messages.Add("Deserialize JSON into InboundEmailModel: success. Length=" + model.Length);
                    report.Log("Deserialize JSON into InboundEmailModel: success");
                    ////report.Message = model;
                }
                else
                {
                    tempModel.Messages.Add("Deserialize JSON into InboundEmailModel: failure");
                    report.Error("Deserialize JSON into InboundEmailModel: failure");

                    var now = DateTime.UtcNow;
                    var nameInvariant = now.ToString("yyyyMMddTHHmmssFFFFFFZ") + "-Mandrill";
                    LogInboundReportToFile(
                        report, 
                        InboundLogType.Raw | InboundLogType.Reply | InboundLogType.Noreply, 
                        nameInvariant,
                        null, tempModel);

                    this.Response.StatusCode = 400;
                    return this.Json(new
                    {
                        Success = false,
                        ErrorCode = "InvalidModel",
                        ErrorMessage = default(string),
                    });
                }

                // Sort the inbound mails in order to initialize services a minimum amount of time
                model = model
                    .OrderBy(o => (o.Msg != null && o.Msg.Email != null) ? new SrkToolkit.Common.Validation.EmailAddress(o.Msg.Email).AccountPart : string.Empty)
                    .ThenBy(o => o.Ts)
                    .ToArray();

                var results = new BasicResult[model.Length];
                for (int i = 0; i < model.Length; i++)
                {
                    report.Clear();
                    report.Log("Deserialize JSON into InboundEmailModel: success");

                    IServiceFactory services = null;
                    try
                    {
                        var currentModel = model[i];

                        EmailAddress email = null;

                        report.RawJson = JsonConvert.SerializeObject(currentModel, Formatting.Indented);
                        if (currentModel.Event == WebHookEventType.Inbound)
                        ////if (currentModel.Event != null && currentModel.Event.ToLowerInvariant() == "inbound")
                        {
                            email = new SrkToolkit.Common.Validation.EmailAddress(currentModel.Msg.Email);
                            report.Message = currentModel.Msg;
                            results[i] = new BasicResult();

                            {
                                var message = "MSG[" + i + "] Inbound email details. Action=" + action
                                    + Environment.NewLine + "From:    " + currentModel.Msg.FromEmail
                                    + Environment.NewLine + "To:      " + string.Join(", ", currentModel.Msg.To.SelectMany(x => x))
                                    + Environment.NewLine + "Subject: " + currentModel.Msg.Subject;
                                tempModel.Messages.Add(message);
                                report.Log(message);
                            }

                            // Email is automatic response, ignore it
                            if (model[i].Msg.Headers.ContainsKey("Auto-Submitted") && (string)model[i].Msg.Headers["Auto-Submitted"] != "no"
                             || model[i].Msg.Subject.ContainsAny(StringComparison.InvariantCultureIgnoreCase, autoReplySubjects))
                            {
                                tempModel.Messages.Add("MSG[" + i + "] Email is of automatic type, no treating done. Subject='" + model[i].Msg.Subject + "'");
                                report.Log("MSG[" + i + "] Email is of automatic type, no treating done. Subject='" + model[i].Msg.Subject + "'");

                                var now = DateTime.UtcNow;
                                var nameInvariant = now.ToString("yyyyMMddTHHmmssFFFFFFZ") + "-Mandrill";
                                LogInboundReportToFile(
                                    report,
                                    InboundLogType.Raw | InboundLogType.Reply,
                                    nameInvariant,
                                    networkName, tempModel);

                                continue;
                            }
                        }
                        else
                        {
                            tempModel.Messages.Add("MSG[" + i + "] hook event='" + currentModel.Event + "' type='" + currentModel.Type + "' is not supported");
                            report.Log("MSG[" + i + "] hook event='" + currentModel.Event + "' type='" + currentModel.Type + "' is not supported");
                        }

                        // Initialize Services
                        if (email != null && (/*services == null || */networkName == null || networkName != email.AccountPart))
                        {
                            networkName = email.AccountPart;
                            if (networkName == "notification" || networkName == "support")
                            {
                                tempModel.Messages.Add("MSG[" + i + "] Email is of " + networkName + " type, no treating done.");
                                report.Log("Email is of " + networkName + " type, no treating done.");

                                var now = DateTime.UtcNow;
                                var nameInvariant = now.ToString("yyyyMMddTHHmmssFFFFFFZ") + "-Mandrill";
                                LogInboundReportToFile(
                                    report,
                                    InboundLogType.Raw | InboundLogType.Reply,
                                    nameInvariant,
                                    networkName, tempModel);

                                continue;
                            }

                            services = InitializeServices(networkName, currentModel.Msg, report);
                            if (services != null)
                            {
                                string message = "InitializeServices: success. "
                                    + Environment.NewLine + "IN:  NetworkName=" + networkName
                                    + Environment.NewLine + "OUT: NetworkId=" + services.NetworkId + ",NetworkName=" + services.Network.Name + ",UniverseName=" + services.AppConfiguration.Application.UniverseName;
                                tempModel.Messages.Add("MSG[" + i + "] " + message);
                                report.Log(message);
                            }
                            else
                            {
                                string message = "InitializeServices: failure. "
                                    + Environment.NewLine + "IN:  NetworkName=" + networkName
                                    + Environment.NewLine + "OUT: services=null";
                                tempModel.Messages.Add("MSG[" + i + "] " + message);
                                report.Error(message);

                                var now = DateTime.UtcNow;
                                var nameInvariant = now.ToString("yyyyMMddTHHmmssFFFFFFZ") + "-Mandrill";
                                LogInboundReportToFile(
                                    report,
                                    InboundLogType.Raw | InboundLogType.Reply | InboundLogType.Noreply | InboundLogType.Html | InboundLogType.Main,
                                    nameInvariant,
                                    networkName, tempModel);
                                results[i].Errors.Add(new BasicResultError("No universe with domain name '" + networkName + "'"));
                            }
                        }

                        if (LocalConfig.HandleInboundEmails && action == MandrillInboundAction.Email && services != null && services.AppConfiguration != null && services.Application != null)
                        {
                            // Email handling
                            var tmpReport = report;
                            report = services.Email.HandleInboundEmail(model[i]);
                            report.Entries = tmpReport.Entries.Concat(report.Entries).ToList();
                            report.ActionLevel += tmpReport.ActionLevel;
                            report.RawJson = tmpReport.RawJson;

                            tempModel.Messages.Add("MSG[" + i + "] HandleInboundEmail: [" + i + "] " + (report.Succeed ? "success" : "failure"));
                            tempModel.Messages.Add("Reason is: " + report.Entries.Last());

                            if (report.Succeed)
                                report.Log("HandleInboundEmail: [" + i + "] success");
                            else
                                report.Error("HandleInboundEmail: [" + i + "] failure");

                            results[i].Succeed = report.Succeed;
                            results[i].Errors.AddRange(report.Entries.Select(o => new BasicResultError(o)).ToList());

                            // Log email
                            var now = DateTime.UtcNow;
                            var nameInvariant = now.ToString("yyyyMMddTHHmmssFFFFFFZ") + "-Mandrill";
                            var flag = InboundLogType.Raw | InboundLogType.Reply | InboundLogType.Html | InboundLogType.Main;
                            if (!report.Succeed)
                                flag |= InboundLogType.Noreply;
                            LogInboundReportToFile(
                                report,
                                flag,
                                nameInvariant,
                                networkName, tempModel);
                            var logId = services.InboundEmailMessages.AddMandrillEmailLog(report.Message, nameInvariant, now, report.Succeed ? true : false);
                            if (report.Succeed)
                                services.InboundEmailMessages.LinkMandrillLogToWallItem(logId, report.OnSucceedPublishId, report.OnSucceedNewItem, report.OnSucceedItemType);
                        }
                        else
                        {
                            tempModel.Messages.Add("MSG[" + i + "] HandleInboundEmail: [" + i + "] not handling message. LocalConfig.HandleInboundEmails=" + LocalConfig.HandleInboundEmails + " action=" + action + " services=" + (services != null ? "set" : "null") + " services.Configuration=" + (services != null && services.AppConfiguration != null ? "set" : "null") + " Subject='" + (model[i].Msg != null ? model[i].Msg.Subject : "null") + "'");
                        }
                    }
                    catch (Exception ex)
                    {
                        results[i] = new BasicResult();
                        results[i].Errors.Add(new BasicResultError(ex.ToString()));
                        tempModel.Messages.Add("MSG[" + i + "] ERROR: " + ex.ToString());
                        report.Error("ERROR: " + ex.ToString());

                        var now = DateTime.UtcNow;
                        var nameInvariant = now.ToString("yyyyMMddTHHmmssFFFFFFZ") + "-Mandrill";
                        LogInboundReportToFile(
                            report,
                            InboundLogType.Raw | InboundLogType.Reply | InboundLogType.Noreply | InboundLogType.Html | InboundLogType.Main,
                            nameInvariant,
                            networkName, tempModel);
                        if (services != null)
                            services.InboundEmailMessages.AddMandrillEmailLog(report.Message, nameInvariant, now, false);
                        HttpErrorReport.Do(this.HttpContext, true, ex, DateTime.UtcNow, this.GetType().Assembly, "HooksController.InboundEmail results[" + i + "]");
                    }
                    finally
                    {
                        if (services != null)
                            services.Dispose();
                    }
                }

                return this.Json(new
                {
                    Success = results.All(r => r!= null && r.Succeed),
                    ErrorCode = default(string),
                    ErrorMessage = default(string),
                    Data = results,
                });
            }
            catch (Exception ex)
            {
                tempModel.Messages.Add(ex.ToString());
                report.Error(ex.ToString());

                var now = DateTime.UtcNow;
                var nameInvariant = now.ToString("yyyyMMddTHHmmssFFFFFFZ") + "-Mandrill";
                var flag = InboundLogType.Noreply;
                if (report.RawJson != null)
                    flag |= InboundLogType.Raw;
                if (report.Message != null)
                    flag |= InboundLogType.Main | InboundLogType.Html;
                if (report.Entries.Count > 0)
                    flag |= InboundLogType.Reply;
                LogInboundReportToFile(report, flag, nameInvariant, networkName, tempModel);

                HttpErrorReport.Do(this.HttpContext, true, ex, DateTime.UtcNow, this.GetType().Assembly, "HooksController.InboundEmail error");
                return this.Json(new
                {
                    Success = false,
                    ErrorCode = "InternalError",
                    ErrorMessage = ex.Message,
                    ErrorTrace = ex.ToString(),
                });
            }
        }

        private HttpRequestModel AddToTemporaryData(string contentString, byte[] contentBytes)
        {
            var collection = this.HttpContext.Application["inbound"] as List<HttpRequestModel>;
            if (collection == null)
            {
                collection = new List<HttpRequestModel>();
                HttpContext.Application["inbound"] = collection;
            }

            var model = HttpRequestModel.Create(this.Request);
            model.PostBytes = contentBytes;
            string json = null;
            if (contentString.StartsWith(MandrilPrefix) && false)
            {
                json = HttpUtility.UrlDecode(contentString.Substring(MandrilPrefix.Length));
                try
                {
                    var obj = JsonConvert.DeserializeObject(json);
                    json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                }
                catch (Exception ex)
                {
                    model.Messages.Add(ex.ToString());
                }
            }

            model.PostContent = json ?? contentString;

            if (this.IsTemporaryDataEnabled)
            {
                collection.Add(model);
            }

            return model;
        }

        private InboundEmailModel[] JsonToInboundModel(string jsonStr, HttpRequestModel tempModel, MandrillInboundAction action)
        {
            InboundEmailModel[] model = null;
            var json = jsonStr ?? "";
            if (jsonStr.StartsWith(MandrilPrefix))
            {
                json = json.Replace("+", " ");
                json = Uri.UnescapeDataString(json);
                ////json = HttpUtility.HtmlDecode(json);
                json = json.Substring(MandrilPrefix.Length);
            }
            else
            {
                tempModel.Messages.Add("JsonToInboundModel: no prefix in string");
            }

            ////tempModel.Messages.Add("JsonToInboundModel: " + json.Substring(0, 40) + "..." + json.Substring(json.Length - 40));

            model = JsonConvert.DeserializeObject<InboundEmailModel[]>(json);

            return model;
        }

        private IServiceFactory InitializeServices(string networkName, InboundEmailMessage msg, InboundEmailReport report)
        {
            SparkleNetworksApplication app = null;
            IServiceFactory services = null;

            // Initialize Services and its attributs
            try
            {
                app = SparkleNetworksApplication.WebCreate(networkName, () => new DefaultEmailTemplateProvider());
                services = app.GetNewServiceFactory(new AspnetServiceCache(this.HttpContext.Cache));
            }
            catch (Exception ex)
            {
                report.Error(ex.Message);
                return null;
            }

            services.HostingEnvironment.LogBasePath = "Sparkle.WebStatus";
            services.HostingEnvironment.RemoteClient = this.Request.UserHostAddress;
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;

            // In case network is down or disabled
            if (app.Config.Application.MainStatusValue < 1 && app.Config.Application.MainStatus != ApplicationStatus.DownForMaintenance)
            {
                services.Logger.Error(
                    "HooksController.InboundEmail",
                    ErrorLevel.Business,
                    "Received email from '{0}' to '{1}' with subject '{2}' but ignored it because app status is {3}",
                    msg.FromEmail, msg.Email, msg.Subject, app.Config.Application.MainStatus.ToString());
                return null;
            }

            services.Application = app.Config.Application;

            return services;
        }

        private void LogInboundReportToFile(InboundEmailReport report, InboundLogType logType, string fileNameInvariant, string emailbox, HttpRequestModel tempModel)
        {
            if (emailbox != null && !System.IO.Directory.Exists(BasePath + emailbox))
                System.IO.Directory.CreateDirectory(BasePath + emailbox);

            var folderPath = emailbox != null ? (BasePath + emailbox) : BasePath ;
            var completePath = Path.Combine(folderPath, fileNameInvariant);

            if ((logType & InboundLogType.Main) == InboundLogType.Main)
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Write -main.html file
                string filePath = completePath + "-Main.html";
                logFiles.Add(filePath);
                tempModel.Files.Add(filePath);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write,  FileShare.None))
                using (var fs = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    fs.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    fs.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
                    fs.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">");
                    fs.WriteLine("\t<head></head>");
                    fs.WriteLine("\t<body>");
                    fs.WriteLine("\t\t<div class=\"headers\">");
                    if (report.Message != null && report.Message.Headers != null)
                    {
                        foreach (var head in report.Message.Headers)
                        {
                            fs.WriteLine("\t\t\t<pre>");
                            fs.WriteLine("\t\t\t\t<code class=\"name\">" + head.Key.ProperHtmlEscape() + "</code>");
                            if (head.Value is JArray)
                            {
                                foreach (var item in ((JArray)head.Value).Select(o => (string)o).ToArray())
                                {
                                    fs.WriteLine("\t\t\t\t<code class=\"value\">" + item.ProperHtmlEscape() + "</code>");
                                }
                            }
                            else
                            {
                                fs.WriteLine("\t\t\t\t<code class=\"value\">" + ((string)head.Value).ProperHtmlEscape() + "</code>");
                            }

                            fs.WriteLine("\t\t\t</pre>");
                        }
                    }

                    fs.WriteLine("\t\t</div>");
                    fs.WriteLine("\t\t<div class=\"text-version\">");
                    if (report.Message != null)
                        fs.WriteLine("\t\t\t<pre>" + report.Message.Text.ProperHtmlEscape() + "</pre>");
                    fs.WriteLine("\t\t</div>");
                    fs.WriteLine("\t</body>");
                    fs.WriteLine("</html>");
                    fs.Flush();
                }
            }

            if ((logType & InboundLogType.Html) == InboundLogType.Html && report.Message != null && report.Message.Html != null)
            {
                // Write -html.html file
                string filePath = completePath + "-html.html";
                logFiles.Add(filePath);
                tempModel.Files.Add(filePath);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var fs = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    fs.Write(report.Message.Html);
                    fs.Flush();
                }
            }

            if ((logType & InboundLogType.Raw) == InboundLogType.Raw && report.RawJson != null)
            {
                // Write -raw.bin file
                string filePath = completePath + "-raw.bin";
                logFiles.Add(filePath);
                tempModel.Files.Add(filePath);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var fs = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    fs.Write(report.RawJson);
                    fs.Flush();
                }
            }

            if ((logType & InboundLogType.Reply) == InboundLogType.Reply && report.Entries != null )
            {
                // Write -Reply.html
                string filePath = completePath + "-Reply.html";
                logFiles.Add(filePath);
                tempModel.Files.Add(filePath);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var fs = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    fs.WriteLine("<pre>");
                    foreach (var item in report.Entries)
                        fs.WriteLine(item);
                    fs.WriteLine("</pre>");
                    fs.Flush();
                }
            }

            if ((logType & InboundLogType.Noreply) == InboundLogType.Noreply)
            {
                // If needed, write -noreply file
                string filePath = completePath + "-noreply";
                logFiles.Add(filePath);
                tempModel.Files.Add(filePath);
                using (new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                }
            }
        }

        private void InitializeInbound()
        {
            return;
            // see Issue #7
            var universes = AppConfiguration.GetUniversesFromConfiguration();
            var apps = AppConfiguration.CreateManyFromConfiguration(universes.Select(x => x.UniverseName));

            foreach (var app in apps)
            {
#warning unfinished code
            }
        }
    }
}
