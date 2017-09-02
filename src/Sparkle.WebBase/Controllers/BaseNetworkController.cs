
namespace Sparkle.WebBase
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Sparkle.Data.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using SrkToolkit.Web;

    /// <summary>
    /// Main sparkle controller with session logic and stuff.
    /// </summary>
    public class BaseNetworkController : Controller
    {
        private IRepositoryFactory repositoryFactory;
        private IServiceFactory serviceFactory;
        private ILogger logger;
        private CultureInfo culture;
        private CultureInfo uiCulture;
        private NetworkSessionService sessionService;
        private TimeZoneInfo timezone;

        protected BaseNetworkController()
            : base()
        {
        }

        protected IRepositoryFactory Repositories
        {
            [DebuggerStepThrough]
            get { return this.Services.Repositories; }
        }

        /// <summary>
        /// Get the DOMAIN LAYER entry point INSTANCE associated with the current HTTP request.
        /// </summary>
        protected IServiceFactory Services
        {
            [DebuggerStepThrough]
            get
            {
                var services = this.serviceFactory ?? (this.HttpContext.GetNetworkServices());

                return services;
            }
        }

        protected ILogger Logger
        {
            [DebuggerStepThrough]
            get { return this.logger ?? (this.logger = this.Services.Logger); }
        }

        protected NetworkMvcApplication Application { get; set; }

        /// <summary>
        /// Gets the CultureInfo to use for display.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                if (this.culture != null)
                    return this.culture;

                this.culture = CultureInfo.CurrentCulture;

                return this.culture;
            }
        }

        /// <summary>
        /// Gets the CultureInfo to use when loading resources.
        /// </summary>
        public CultureInfo UICulture
        {
            get { return this.Culture; }
        }

        /// <summary>
        /// Provides access to a high-level strongly-typed session state object.
        /// </summary>
        public NetworkSessionService SessionService
        {
            [DebuggerStepThrough]
            get
            {
                if (this.sessionService == null)
                {
                    this.sessionService = new NetworkSessionService(this.Session);
                    this.sessionService.ReviveIfDead(() => this.Services, this.HttpContext);
                }

                return this.sessionService;
            }
        }

        public int? UserId
        {
            [DebuggerStepThrough]
            get
            {
                var user = this.SessionService.User;
                if (user != null)
                {
                    return user.Id;
                }

                // TODO: remove this obsolete code
                user = UserContext.Me;
                if (user != null)
                {
                    return user.Id;
                }

                return null;
            }
        }

        public string Username
        {
            get { return this.User != null ? this.User.Identity != null ? this.User.Identity.Name != null ? this.User.Identity.Name : null : null : null; }
        }

        public TimeZoneInfo Timezone
        {
            get { return this.Services.Context.Timezone; }
        }

        public Sparkle.Services.Authentication.MembershipUser CurrentUser { get; set; }

        public void Initialize(Controller controller)
        {
            this.Initialize(controller.ControllerContext.RequestContext);
        }

        public void InitializeEx(RequestContext requestContext)
        {
            this.Initialize(requestContext);
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            this.Application = this.HttpContext.ApplicationInstance as NetworkMvcApplication;
            this.ExtraInitialize();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.Services.HostingEnvironment.LogBasePath = (this.RouteData.Values["controller"] ?? "Home?") + "/" + (this.RouteData.Values["action"] ?? "Action?");
            this.Services.HostingEnvironment.RemoteClient = this.Request.UserHostAddress;

            if (this.Services.HostingEnvironment.Identity == null)
            {
                // temporary for logging
                this.Services.HostingEnvironment.Identity = ServiceIdentity.Anonymous;
            }

            if (this.Session != null)
            {
                var me = this.SessionService.User;
                if (me != null)
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(me.Timezone ?? this.Services.AppConfiguration.Tree.Features.I18N.DefaultTimezone ?? "Romance Standard Time");

                    this.Services.Context.UserId = me.Id;
                    this.Services.Context.Timezone = tz;
                    this.Services.HostingEnvironment.Identity = ServiceIdentity.User(me);
                }
                else
                {
                    this.Services.HostingEnvironment.Identity = ServiceIdentity.Anonymous;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            this.InitializeViewData(this.ViewData);

            base.OnResultExecuting(filterContext);

            if (Request.Url != null)
                this.ViewData["AlternativeSite"] = this.Application.Config.Tree.Site.MainDomainName != Request.Url.Host;
            else
                this.ViewData["AlternativeSite"] = false;
        }

        protected void InitializeViewData(ViewDataDictionary viewData)
        {
            viewData["Timezone"] = this.Timezone;
            var viewServices = new NetworkViewServices
            {
                Session = this.Session != null ? this.SessionService : null,
                Culture = this.Culture,
                UICulture = this.UICulture,
                Timezone = this.Timezone,
                AppConfigTree = this.Services.AppConfiguration.Tree,
                CurrentNetwork = this.Services.Network.Clone(),
#if DEBUG
                DebugBuild = true,
#else
                DebugBuild = false,
#endif
            };
            viewData["ViewServices"] = viewServices;
            HttpContext.Items["ViewServices"] = viewServices;
            ////HttpContext.SetDateTimeFormats(timeFormat: "t");
        }

        public virtual void ExtraInitialize()
        {
            this.Services.HostingEnvironment.Identity = ServiceIdentity.Anonymous;
            this.Services.HostingEnvironment.LogBasePath = this.RouteData.Values["controller"] + "/" + this.RouteData.Values["action"];
            this.Services.HostingEnvironment.RemoteClient = this.Request.UserHostAddress;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Application = null;
                if (this.serviceFactory != null)
                {
                    this.serviceFactory.Dispose();
                    this.serviceFactory = null;
                }
                else if (this.repositoryFactory != null)
                {
                    this.repositoryFactory.Dispose();
                    this.repositoryFactory = null;
                }

                this.logger = null;
            }

            base.Dispose(disposing);
        }

        protected string T(string value)
        {
            return this.Services.Lang.T(value);
        }

        protected string T(string value, params object[] parameters)
        {
            return this.Services.Lang.T(value, parameters);
        }

        protected string M(string singularValue, string pluralValue, int count)
        {
            return this.Services.Lang.M(singularValue, pluralValue, count);
        }

        protected string M(string singularValue, string pluralValue, int count, params object[] parameters)
        {
            return this.Services.Lang.M(singularValue, pluralValue, count,parameters);
        }
    }

    public static class NetworkControllerExtensions
    {
        /// <summary>
        /// Builds an URL from the specified controller, action, route, values...
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <param name="route">The route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns></returns>
        public static string GetSimpleUrl(this Controller ctrl, string controller, string action, string route = "Default", object routeValues = null)
        {
            return UrlHelper.GenerateUrl(
                route,
                action,
                controller,
                routeValues != null ? new RouteValueDictionary(routeValues) : new RouteValueDictionary(),
                RouteTable.Routes,
                ctrl.ControllerContext.RequestContext,
                true);
        }

        public static string AnyLocalUrl(this Controller ctrl, string url1, bool tryReferer, string url2 = null)
        {
            if (url1 != null && ctrl.Url.IsLocalUrl(url1))
                return url1;

            if (tryReferer && ctrl.Request != null && ctrl.Request.UrlReferrer != null)
            {
                string referer = ctrl.Request.UrlReferrer.PathAndQuery;
                if (ctrl.Url.IsLocalUrl(referer))
                    return referer;
            }

            if (url2 != null && ctrl.Url.IsLocalUrl(url2))
                return url2;

            return null;
        }
    }
}
