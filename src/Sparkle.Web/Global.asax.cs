
namespace Sparkle
{
    using BundleTransformer.Core.Bundles;
    using BundleTransformer.Core.Orderers;
    using Sparkle.App_Start;
    using Sparkle.Controllers;
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.EmailTemplates;
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Helpers;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Constants;
    using Sparkle.Services.Main;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.WebBase;
    using SrkToolkit.Web.HttpErrors;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration.Provider;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Profile;
    using System.Web.Routing;
    using System.Web.Security;

    public class MvcApplication : NetworkMvcApplication
    {
        private static readonly object globalFiltersLock = new object();

        /// <summary>
        /// do not error report when an error occurs at those locations
        /// </summary>
        static private readonly string[] ignoreErrorUrls = new string[]
        {
            // internal stuff
            "/null",                       // maps bugs
            "/Events/Event/null",          // maps bugs
            "/Event/null",                 // maps bugs
            "/Group/null",                 // ???
            "/%3Ca%20href=",//"/<a href=", // markdown bug
            "/SideBar/Live",               // high-frequency pages
            "/SideBar/GetMessages",        // high-frequency pages
            "/uploadify/uploadify.swf",    // so many bots try to locate this file (we don't have it)
            "/scripts/javascript:false;",  // bot bug?
            "/index.php",                  // no, there is no index.php
            "/bundles/javascript:false;",  // bot bug?
            "/welcome'",                   // bot bug
        };

        /// <summary>
        /// do not error report when an error occurs at those locations
        /// </summary>
        static private readonly Func<string, bool>[] ignoreErrorRules = new Func<string, bool>[]
        {
            // internal stuff
            s => s.StartsWith("/<a"),
            s => s.StartsWith("/<noscript>"),
            s => s.StartsWith("/Content/Site/menu_") && s.EndsWith(".png"),
            s => s.StartsWith("/Event/") && s.EndsWith("/trackback/"),
            s =>
            {
                var badChars = new char[] { ')', ']', '}', ',', };
                return s.StartsWith("/b/Styles/") && s.Length > 10 && badChars.Contains(s[10]);
            },
            s => s.StartsWith("/m/Company/"),
        };

        protected override void Application_Start_Mvc()
        {
            base.Application_Start_Mvc();

            // minify our bundles? only in RELEASE configuration
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
            BundleConfig.ConfigureStaticBundles(BundleTable.Bundles);
        }

        protected override void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            base.RegisterGlobalFilters(filters);
        }

        protected override void ConfigureDependencies()
        {
            base.ConfigureDependencies();

            // Enables action methods to send and receive JSON-formatted text and to model-bind
            // the JSON text to parameters of action methods.
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());

            // remove the XML serializer from the webapi because it seems useless
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            // remove useless view engines
            foreach (var engine in ViewEngines.Engines.ToArray())
            {
                if (engine is WebFormViewEngine && ViewEngines.Engines.Contains(engine))
                {
                    ViewEngines.Engines.Remove(engine);
                }
            }
        }

        protected override void RegisterIgnoreRoutes(RouteCollection routes)
        {
            ////base.RegisterIgnoreRoutes(routes);
            var ignore = new List<string>()
            {
                "{favicon}.ico",
                "null",
            };
            DefaultIgnoreRoutes
                .Where(r => !ignore.Contains(r))
                .ToList()
                .ForEach(r => routes.IgnoreRoute(r));
        }

        protected override void RegisterMainRoutes(RouteCollection routes)
        {
            base.RegisterMainRoutes(routes);

            var mainNamespaces = new string[] { "Sparkle.Controllers" };
            routes.MapHttpRoute(
                name: "Api 1.0",
                routeTemplate: "api/1.0/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, controller = "Api10", });
            routes.MapRoute("favicon.ico", "favicon.ico", new { controller = "Data", action = "Favicon" }, mainNamespaces);
            routes.MapRoute("robots.txt", "robots.txt", new { controller = "Data", action = "Robots" }, mainNamespaces);
            routes.MapRoute("Index", "", new { controller = "Home", action = "News" }, mainNamespaces);
            routes.MapRoute("Home", "Home", new { controller = "Home", action = "Index" }, mainNamespaces);
            routes.MapRoute("Welcome", "Welcome", new { controller = "Home", action = "Welcome" }, mainNamespaces);
            routes.MapRoute("Conversations", "Conversations/{id}", new { controller = "Messages", action = "Index", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Person", "Person/{id}", new { controller = "Peoples", action = "People", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Company", "Company/{id}", new { controller = "Companies", action = "Company", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("CompanyKind", "CompanyKind/{id}", new { controller = "CompanyKinds", action = "Details", }, mainNamespaces);
            routes.MapRoute("Job", "Job/{id}", new { controller = "Jobs", action = "Job", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Club", "Club/{id}", new { controller = "Clubs", action = "Club", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Event", "Event/{id}", new { controller = "Events", action = "Event", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Place", "Place/{id}", new { controller = "Places", action = "Place", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Group", "Group/{id}", new { controller = "Groups", action = "Group", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Project", "Project/{id}", new { controller = "Projects", action = "Project", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Tag", "Tags/Details/{category}/{id}", new { controller = "Tags", action = "Details", }, mainNamespaces);
            routes.MapRoute("Calendar-Main", "Calendar.ics/{action}/{username}/{key}", new { controller = "Calendar" }, mainNamespaces);
            routes.MapRoute("Calendars", "Cal/{username}/{key}/{action}/{displayName}", new { controller = "Calendar", displayName = string.Empty }, mainNamespaces);
            routes.MapRoute("AccountRecovery", "Account/Recovery/{id}/{key}", new { controller = "Account", action = "Recovery" }, mainNamespaces);
            routes.MapRoute("EmailChange", "Account/EmailChangeConfirmed/{id}/{key}", new { controller = "Account", action = "EmailChangeConfirmed" }, mainNamespaces);
            routes.MapRoute("DataPicture", "Data/{action}/{id}/{size}/{date}", new { controller = "Data", size = "Medium", date = UrlParameter.Optional, }, mainNamespaces);
            routes.MapRoute("LunchPlan", "Lunch/Plan/{id}/{date}", new { controller = "Lunch", action = "Plan", date = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("HelpPage", "Help/Page/{*id}", new { controller = "Help", action = "Page", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Page", "Page/{*id}", new { controller = "Pages", action = "Page", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("NetworkPage", "Pages/NetworkFile/{*id}", new { controller = "Pages", action = "NetworkFile", id = UrlParameter.Optional }, mainNamespaces);
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, mainNamespaces);
        }

        protected override void Session_Start(object sender, EventArgs e)
        {
            base.Session_Start(sender, e);

            if (this.Request.IsAuthenticated)
            {
                // an authenticated user comes back after its session has expired
                // populate the session values beforehand
                this.PopulateSessionValues();
            }
        }

        /// <summary>
        /// Ensure the session contains an instance of the authenticated user.
        /// </summary>
        private void PopulateSessionValues()
        {
            try
            {
                HttpContext httpContext;
#warning we should not use HttpContext.Current
                if ((httpContext = HttpContext.Current) == null || httpContext.Session == null /*|| ServiceFactory.Provider == null*/)
                {
                    Trace.TraceInformation("SparkleMvcApp.PopulateSessionValues: " + (httpContext != null ? "HttpContext.Session" : "HttpContext") + " is null");
                }

                if (httpContext.User != null && httpContext.User.Identity != null && httpContext.User.Identity.Name != null)
                {
                    // an identity in installed. now check the session values.
                    var me = (User)httpContext.Session[SessionConstants.Me];
                    if (me == null)
                    {
                        // the "me" object is not in session. let's put it back!
                        if (me == null || me.Login != httpContext.User.Identity.Name)
                        {
                            var services = httpContext.GetNetworkServices();
                            if (services == null)
                            {
                                Trace.TraceInformation("SparkleMvcApp.PopulateSessionValues: services is null");
                            }
                            else
                            {
                                try
                                {
                                    string login = httpContext.User.Identity.Name;
                                    if (string.IsNullOrEmpty(login) && me != null)
                                        login = me.Login;
                                    if (string.IsNullOrEmpty(login))
                                        return;

                                    var me2 = services.People.GetForSessionByLogin(login);
                                    httpContext.Session[SessionConstants.Me] = me2;
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceError("SparkleMvcApp.PopulateSessionValues: error retreiving data" + Environment.NewLine + ex.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("SparkleMvcApp.PopulateSessionValues: unknown error" + Environment.NewLine + ex.ToString());
            }
        }

        protected override void LoadApplicationResources(AppConfiguration config)
        {
            base.LoadApplicationResources(config);
            this.ConfigureLangSource();

            var networkName = config.Tree.NetworkName ?? config.Application.UniverseName;
            BundleConfig.ConfigureNetworkBundles(BundleTable.Bundles, this.Server, networkName);
        }

        protected override void ReconfigureInternalProviders(string connectionString)
        {
            base.ReconfigureInternalProviders(connectionString);

            // Configure the MEMBERSHIP provider to authenticate user againt the correct database.
            // Normal people put the connection string in the Web.config file.
            // Here we cannot because the configuration is given at runtime by sparklesystems.
            try
            {
                // HACK: setting private fields by reflection. this is because we can't alter the cxx string that is within the membership provider
                var connectionStringField = Membership.Provider
                        .GetType()
                        .GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
                if (connectionStringField != null)
                    connectionStringField.SetValue(Membership.Provider, connectionString);
            }
            catch (ProviderException)
            {
                // in case membership is simply disabled
            }

            // Configure the ROLES provider to authenticate user againt the correct database.
            // we do not use this thing any more.
            if (Roles.Enabled)
            {
                // HACK: setting private fields by reflection. this is because we can't alter the cxx string that is within the roles provider
                var roleField = Roles.Provider
                    .GetType()
                    .GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
                if (roleField != null)
                    roleField.SetValue(Roles.Provider, connectionString);
            }

            // Configure the PROFILES provider to authenticate user againt the correct database.
            // we do not use this thing any more.
            if (ProfileManager.Enabled)
            {
                // HACK: setting private fields by reflection. this is because we can't alter the cxx string that is within the profiles provider
                var profileField = ProfileManager.Provider
                    .GetType()
                    .GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
                if (profileField != null)
                    profileField.SetValue(ProfileManager.Provider, connectionString);
            }
        }

        protected override IErrorController GetNewErrorController()
        {
            return new Controllers.ErrorController();
        }

        /// <summary>
        /// Returns a value that says whether the current error is NOT worth an error report.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected override bool IsHttpErrorToBeIgnored(HttpContext context, Exception exception)
        {
            string url = context.Request.Url.PathAndQuery;

            if (url != null && ignoreErrorUrls.Contains(url))
            {
                return true;
            }

            if (url != null && ignoreErrorRules.Any(rule => rule(url)))
            {
                return true;
            }

            return base.IsHttpErrorToBeIgnored(context, exception);
        }

        protected override void Application_EndRequest()
        {
            // this is the end of the request. destroy the associated objects.
            var service = this.Context.GetNetworkServices(false);
            if (service != null)
            {
                service.Dispose();
                if (service.Logger != null)
                    service.Logger.Dispose();

                this.Context.ClearNetworkServices();
            }

            base.Application_EndRequest();
        }

        protected override void OnApplicationConfigured(AppConfiguration config)
        {
            base.OnApplicationConfigured(config);

            // if the Subscriptions system is active, install a global filter that will prevent non-subscribed users from seeing anything
            // the few allowed pages override this using a SubscriberAcces attribute
            lock (globalFiltersLock) // thread safety is required here
            {
                if (config.Tree.Features.Subscriptions.IsEnabled && !GlobalFilters.Filters.OfType<SubscriberAccessAttribute>().Any())
                {
                    GlobalFilters.Filters.Add(new SubscriberAccessAttribute(RequiresActiveSubscription: true));
                } 
            }

            // add theming support
            // we need to overwrite the list of view locations to prioritize themed views
            var viewEngines = ViewEngines.Engines;
            var razor = viewEngines.OfType<RazorViewEngine>().SingleOrDefault();
            var themeName = config.Tree.Site.ThemeName;
            if (razor != null && themeName != null)
            {
                var list = razor.MasterLocationFormats
                    .Where(x => !x.EndsWith(".vbhtml")) // don't search for this kind of file
                    .ToList();
                list.Insert(0, "~/Themes/" + themeName + "/Views/{1}/{0}.cshtml");    // preprend the list with themes locations
                list.Insert(1, "~/Themes/" + themeName + "/Views/Shared/{0}.cshtml"); // preprend the list with themes locations
                razor.MasterLocationFormats = list.ToArray();

                list = razor.PartialViewLocationFormats
                    .Where(x => !x.EndsWith(".vbhtml")) // don't search for this kind of file
                    .ToList();
                list.Insert(0, "~/Themes/" + themeName + "/Views/{1}/{0}.cshtml");    // preprend the list with themes locations
                list.Insert(1, "~/Themes/" + themeName + "/Views/Shared/{0}.cshtml"); // preprend the list with themes locations
                razor.PartialViewLocationFormats = list.ToArray();

                list = razor.ViewLocationFormats 
                    .Where(x => !x.EndsWith(".vbhtml")) // don't search for this kind of file
                    .ToList();
                list.Insert(0, "~/Themes/" + themeName + "/Views/{1}/{0}.cshtml");    // preprend the list with themes locations
                list.Insert(1, "~/Themes/" + themeName + "/Views/Shared/{0}.cshtml"); // preprend the list with themes locations
                razor.ViewLocationFormats = list.ToArray();
            }
        }
    }
}
