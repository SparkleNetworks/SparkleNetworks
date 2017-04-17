
namespace Sparkle.WebBase
{
    using Sparkle.Helpers;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Constants;
    using Sparkle.Infrastructure.Data;
    using Sparkle.Services.Networks;
    using Sparkle.UI;
    using SrkToolkit.Web;
    using SrkToolkit.Web.HttpErrors;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Configuration.Provider;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Profile;
    using System.Web.Routing;
    using System.Web.Security;

    /// <summary>
    /// Base application class for Sparkle websites. In charge of initilizing the app, loading resources,
    /// handling errors, handling routes.
    /// </summary>
    public abstract class NetworkMvcApplication : BaseMvcApplication
    {
        private readonly object initializeLock = new object();
        private volatile bool initialized;
        private DateTime? appStartTimeUtc;
        private bool weeklyNewsletterCountersInitialized;
        protected NetworkInfo networkInfo;

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; private set; }

        public AppConfiguration Config { get; protected set; }

        public DateTime AppStartTimeUtc
        {
            get { return this.appStartTimeUtc.HasValue ? this.appStartTimeUtc.Value : (this.appStartTimeUtc = DateTime.UtcNow).Value; }
        }

        /// <summary>
        /// Occurs when the application starts.
        /// Initializes ASP MVC through <see cref="Application_Start_Mvc"/>.
        /// Initializes Data and Services through <see cref="ConfigureDependencies"/>.
        /// </summary>
        protected virtual void Application_Start()
        {
            this.Application_Start_Mvc();
            this.ConfigureDependencies();
        }

        /// <summary>
        /// Occurs when the application starts.
        /// Initializes ASP MVC.
        /// To prevent MVC from initilizing, override this method without calling the base method.
        /// </summary>
        protected virtual void Application_Start_Mvc()
        {
            RegisterGlobalFilters(GlobalFilters.Filters);

            this.RegisterIgnoreRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
            this.RegisterMainRoutes(RouteTable.Routes);
            this.RegisterFinalRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// <see cref="Sparkle.Data.RepositoryFactory.Provider"/> and <see cref="Sparkle.Services.ServiceFactory.Provider"/>
        /// should be set here.
        /// </summary>
        protected virtual void ConfigureDependencies()
        {
            // Change the default JSON serializer to the most popular one
            SrkToolkit.Web.JsonNetResult.Serializer = (obj, response) =>
            {
                Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(response.Output)
                {
#if DEBUG
                    Formatting = Newtonsoft.Json.Formatting.Indented,
#endif
                };
                var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                };
                var serializer = Newtonsoft.Json.JsonSerializer.Create(serializerSettings);
                serializer.Serialize(writer, obj);
                writer.Flush();
            };
        }

        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        protected virtual void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
        }

        protected virtual void RegisterIgnoreRoutes(RouteCollection routes)
        {
            DefaultIgnoreRoutes.ForEach(r => routes.IgnoreRoute(r));
        }

        protected virtual void RegisterMainRoutes(RouteCollection routes)
        {
        }

        protected virtual void RegisterFinalRoutes(RouteCollection routes)
        {
        }

        /// <summary>
        /// Occurs when the application stops.
        /// </summary>
        protected virtual void Application_End()
        {
        }

        /// <summary>
        /// Occurs when an exception is not handled.
        /// Default implementation is to send an error report.
        /// </summary>
        protected virtual void Application_Error()
        {
#if DEBUG
            bool debug = true;
#else
            bool debug = false;
#endif

            Exception exception = null;
            var errorController = this.GetNewErrorController();
            var assembly = errorController != null ? errorController.GetType().Assembly : this.GetType().Assembly;
            var context = this.Context;
            try
            {
                // handle exception
                if (errorController != null)
                {
                    exception = ErrorControllerHandler.Handle(context, errorController, debug);
                    if (exception != null)
                        Trace.TraceError("Application_Error: " + exception.ToString());
                }
                else
                {
                    exception = Server.GetLastError();
                    if (exception != null)
                    {
                        Trace.TraceError("Application_Error: " + exception.ToString());
                        Response.Clear();
                        Response.TrySkipIisCustomErrors = true;
                        BasicHttpErrorResponse.Execute(this.Context, exception, message: "Warning: No error controller defined.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (exception == null)
                    exception = Server.GetLastError();

                HttpErrorReport.Do(this.Context, false, new Exception[] { exception, ex, }, appStartTimeUtc.Value, assembly, default(string));
                BasicHttpErrorResponse.Execute(this.Context, exception, extraError: ex, message: "Warning: The error page execution failed on top of another exception.");
            }

            // send error report if necessary
            if (!debug && exception != null)
            {
                if (!this.IsHttpErrorToBeIgnored(context, exception))
                {
                    HttpErrorReport.Do(this.Context, false, new Exception[] { exception, }, appStartTimeUtc.Value, assembly, default(string));
                }
            }

            // break into debugger if desired
#if DEBUG
            if (Debugger.IsAttached)
            {
                var errors = Context.AllErrors;
                Debugger.Break();
            }
#endif
        }

        protected virtual IErrorController GetNewErrorController()
        {
            return null;
        }

        /// <summary>
        /// Occurs when a session starts.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void Session_Start(Object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the request state that is associated with the current request has been obtained.
        /// Default implementation loads the applications resources and sets the current culture.
        /// </summary>
        protected virtual void Application_BeginRequest()
        {
            this.ConfigureApplication(HttpContext.Current);

            if (!this.ForceHttps(this.Config, HttpContext.Current))
            {
                this.ForceMainDomainName(this.Config, HttpContext.Current);
            }
        }

        protected virtual void Application_PostAcquireRequestState()
        {
            this.SetCultureForRequest();

            this.CheckTrackingCodes(HttpContext.Current);
        }

        protected virtual void Application_EndRequest()
        {
        }

        protected virtual void CheckTrackingCodes(HttpContext context)
        {
            HttpRequest request = context.Request;
            var tc = request.QueryString["tc"];
            if (tc != null)
            {
                Sparkle.Services.Networks.IServiceFactory services = null;
                try
                {
                    ////services = Sparkle.Services.Networks.ServiceFactory.New;
                    services = context.GetNetworkServices();
                    if (!this.weeklyNewsletterCountersInitialized)
                    {
                        services.StatsCounters.SetupWeeklyNewsletterCounters();
                        this.weeklyNewsletterCountersInitialized = true;
                    }

                    services.StatsCounters.Hit(tc);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("CheckTrackingCodes encountered an error: " + Environment.NewLine + ex.ToString());
                }
                finally
                {
                    ////if (services != null)
                    ////    services.Dispose();
                }
            }
        }

        private bool ForceHttps(AppConfiguration conf, HttpContext context)
        {
            var request = context.Request;

            // why this code?
            if (string.IsNullOrEmpty(request.Path))
                return false;

            if (request.IsSecureConnection)
                return false;

            if (!this.Config.Tree.Site.ForceSecureHttpGet && !this.Config.Tree.Site.ForceSecureHttpRequests)
                return false;

            if (request.HttpMethod != "GET" && !conf.Tree.Site.ForceSecureHttpRequests)
                return false;

            var domain = conf.Tree.Site.MainDomainName ?? request.Url.Host;
            string url = "https://" + domain + request.RawUrl;
            context.Response.RedirectPermanent(url, true);
            return true;
        }

        private bool ForceMainDomainName(AppConfiguration conf, HttpContext context)
        {
            var request = context.Request;

            // why this code?
            if (string.IsNullOrEmpty(request.Path))
                return false;

            if (request.HttpMethod != "GET")
                return false;

            var domainMatch = conf.DomainNames != null ? conf.DomainNames.FirstOrDefault(d => string.Compare(request.Url.Host, d.Name, true) == 0 && d.RedirectToMain) : null;
            if (conf.Tree.Site.RedirectToMainDomain && string.Compare(request.Url.Host, conf.Tree.Site.MainDomainName, true) == 0
             || domainMatch != null)
            {
                string url = "http://" + conf.Tree.Site.MainDomainName + request.RawUrl;
                context.Response.RedirectPermanent(url, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Configures the application resources.
        /// </summary>
        protected virtual void ConfigureApplication(HttpContext context)
        {
            HttpRequest request = context.Request;
            var host = request.Url.Host;
            if (!this.initialized)
            {
                lock (initializeLock)
                {
                    var app = context.GetSparkleApp();
                    this.Config = app.Config;
                    this.ApplicationName = this.Config.Application.UniverseName;

                    if (this.networkInfo == null)
                    {
                        IServiceFactory services;
                        try
                        {
                            services = this.Context.GetNetworkServices();
                        }
                        catch (Exception ex)
                        {
                            this.Context.ClearSparkleApp();
                            throw;
                        }

                        Sparkle.Entities.Networks.NetworkType networkType = services.Networks.GetNetworkType(services.Network.NetworkTypeId);
                        this.networkInfo = new NetworkInfo
                        {
                            NetworkId = services.NetworkId,
                            NetworkName = services.Network.Name,
                            NetworkTypeId = services.Network.NetworkTypeId,
                            NetworkTypeName = services.Network.Type.Name,
                        };
                    }

                    this.ConfigureLogging(this.Config);
                    this.ConfigureDataAndServices(this.Config);

                    // load POT files
                    this.LoadApplicationResources(this.Config);

                    this.OnApplicationConfigured(this.Config);

                    this.initialized = true;
                    this.appStartTimeUtc = DateTime.UtcNow;
                }
            }
            else
            {
                if (IsValidateDomainName())
                {
                    if (!ValidateApplicationName(host, this.ApplicationName))
                    {
                        throw new HttpException(500, "System configuration error (domain name does not match any application)");
                    }
                }
            }
        }

        /// <summary>
        /// Ensures a request is executed under the right application key.
        /// </summary>
        /// <param name="host">The hostname of the request.</param>
        /// <param name="appName">The actual application name.</param>
        /// <returns></returns>
        private bool ValidateApplicationName(string host, string appName)
        {
            host = host.ToLowerInvariant();
            return this.Config.DomainNames.Any(d => d.Name.ToLowerInvariant() == host);
        }

        protected virtual void OnApplicationConfigured(AppConfiguration config)
        {
        }

        protected virtual void ConfigureLogging(AppConfiguration config)
        {
            }

        protected virtual void ConfigureDataAndServices(AppConfiguration config)
        {
            this.ReconfigureInternalProviders(config.Tree.ConnectionStrings.NetworkApplicationServices);
        }

        /// <summary>
        /// Reconfigures the internal providers (membership).
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        protected virtual void ReconfigureInternalProviders(string connectionString)
        {
        }

        /// <summary>
        /// Loads the application resources.
        /// </summary>
        protected virtual void LoadApplicationResources(AppConfiguration config)
        {
        }

        protected void ConfigureLangSource()
        {
            if (Lang.Source == null)
            {
                var source = Sparkle.Services.Main.Networks.SparkleLang.CreateStrings(
                    HttpContext.Current.Server.MapPath("~/Lang"),
                    this.Config.Application.UniverseName,
                    this.networkInfo != null ? this.networkInfo.NetworkTypeName : null);

                Lang.Source = source;
                Lang.AvailableCultures = source.AvailableCultures;

                if (Lang.T("AppName") == null)
                {
                    throw new InvalidOperationException("ConfigureLangSource execution test failed");
                }
            }
        }

        public class PotLoad
        {
            public PotLoad()
            {
            }

            public PotLoad(string directory, string application, string defaultCulture)
            {
                this.Directory = directory;
                this.Application = application;
                this.Culture = defaultCulture;
            }

            public string Directory { get; set; }
            public string Application { get; set; }
            public string Culture { get; set; }

            public override string ToString()
            {
                return "'" + this.Directory + "/" + this.Application + "/" + this.Culture + "'";
            }
        }

        /// <summary>
        /// Permits to return the prefered user culture.
        /// Returns null by default.
        /// </summary>
        /// <returns></returns>
        protected virtual CultureInfo GetUserCulture(HttpContext context)
        {
            if (context != null && context.Session != null)
            {
                var service = new NetworkSessionService(new HttpSessionStateWrapper(context.Session));
                var user = service.User;
                if (user != null)
                {
                    var browserCultures = CultureTools.GetBrowserCultures(context.Request.UserLanguages);
                    return context.GetNetworkServices().People.GetCulture(user, browserCultures);
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the culture for the current request.
        /// </summary>
        protected virtual void SetCultureForRequest()
        {
            var context = this.Context;
            var supportedCultures = context.GetNetworkServices().SupportedCultures;

            ////var culture = CultureTools.GetSessionCulture(context, Lang.AvailableCultures) ??
            var culture = this.GetUserCulture(context) ??
                          CultureTools.GetCookieCulture(context, supportedCultures.ToList()) ??
                          CultureTools.GetBrowserCulture(context, supportedCultures.ToList()) ??
                          CultureInfo.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = culture;
            CultureTools.SaveClientCulture(context, culture);
        }

        public void ResetInitialize()
        {
            this.initialized = false;
        }

        private static bool IsValidateDomainName()
        {
            bool value;
            var config = ConfigurationManager.AppSettings["SparkleSystems.ValidateDomainName"];
            var useDomainName = "Auto".Equals(ConfigurationManager.AppSettings["SparkleSystems.Universe"], StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(config) && !config.Equals("Auto", StringComparison.OrdinalIgnoreCase) && bool.TryParse(config, out value))
            {
                return value;
            }

            return useDomainName;
        }

        public class NetworkInfo
        {
            public int NetworkId { get; set; }
            public string NetworkName { get; set; }
            public int NetworkTypeId { get; set; }
            public string NetworkTypeName { get; set; }
        }
    }
}
