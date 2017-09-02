
namespace Sparkle.WebBase
{
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Constants;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Web.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Security;

    /// <summary>
    /// Represents a high-level strongly-typed HTTP session for the network website.
    /// </summary>
    public class NetworkSessionService : BaseSessionService
    {
        /// <summary>
        /// Initializes an instance with a HttpSessionStateBase object (likely from ASP MVC).
        /// </summary>
        /// <param name="httpSessionStateBase"></param>
        public NetworkSessionService(HttpSessionStateBase httpSessionStateBase)
            : base(httpSessionStateBase)
        {
        }

        public NetworkSessionService(IDictionary<string, object> httpSessionDictionary)
            : base(httpSessionDictionary)
        {
        }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        public User User
        {
            get { return this.GetObject<User>(SessionConstants.Me); }
            set { this.Set<User>(SessionConstants.Me, value); }
        }

        public DateTime? LoginDate
        {
            get { return this.GetValue<DateTime>("LoginDate"); }
            set
            {
                if (value.HasValue)
                    this.Set<DateTime>("LoginDate", value.Value);
                else
                    this.Clear("LoginDate");
            }
        }

        /// <summary>
        /// Gets or sets a value to enabled JS debugging.
        /// </summary>
        public bool JsDebug
        {
            get
            {
#if DEBUG
                bool debug = true;
#else
                bool debug = true;
#endif
                return this.GetValue<bool>("JsDebug") ?? debug;
            }
            set { this.Set<bool>("JsDebug", value); }
        }

        public void ClearUser()
        {
            this.User = null;
        }

        /// <summary>
        /// Refreshes the session is case it expired. This renews the User property based on the authenticated user ID.
        /// </summary>
        /// <param name="servicesGetter"></param>
        /// <param name="context"></param>
        /// <param name="forcedIdentity"></param>
        public void ReviveIfDead(Func<IServiceFactory> servicesGetter, HttpContextBase context, string forcedIdentity = null)
        {
            if (context.Session == null)
                return;

            // find current username
            string identity = forcedIdentity;
            if (identity == null && context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
                identity = context.User.Identity.Name;

            // no username? quit
            if (identity == null)
            {
                this.User = null;
                return;
            }

            // refresh whatevers needs to be
            Refresh(servicesGetter, context, identity);
        }

        private void Refresh(Func<IServiceFactory> servicesGetter, HttpContextBase context, string forcedIdentity = null)
        {
            // find current username
            string identity = forcedIdentity;
            if (identity == null && context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
                identity = context.User.Identity.Name;

            // no username? quit
            if (identity == null)
                return;

            // verify User.Username == Identity.Name
            if (this.User != null && !this.User.Username.Equals(context.User.Identity.Name, StringComparison.OrdinalIgnoreCase))
            {
                servicesGetter().Logger.Error("SessionService.Refresh", ErrorLevel.Authn, "The forms identity (" + identity + ") differs from the user in session (" + this.User.Username + "). Session cleared and reviving.");
                this.Clear();
            }

            if (this.User != null && this.User.Username == context.User.Identity.Name)
            {
                // session is OK, quit
                return;
            }

            //var user = servicesGetter().People.SelectWithLogin(identity);
            var user = servicesGetter().People.GetForSessionByLogin(identity);
            if (user == null)
            {
                servicesGetter().Logger.Error("SessionService.ReviveIfDead", ErrorLevel.Authn, "The forms identity (" + identity + ") corresponds to no user. Session and identity cleared.");
                this.Clear();
                FormsAuthentication.SignOut();
                context.User = null;
                context.Response.Redirect(context.Request.Url.PathAndQuery);
                return;
            }

            this.User = user;

            servicesGetter().Logger.Info("SessionService.ReviveIfDead", ErrorLevel.Success, "Revived session for identity (" + identity + ").");
        }

        public string LinkedInMatchId
        {
            get { return this.GetObject<string>("LinkedInMatchId"); }
            set { this.Set<string>("LinedInMatchId", value); }
        }

        public string LinkedInMatchEmail
        {
            get { return this.GetObject<string>("LinkedInMatchEmail"); }
            set { this.Set<string>("LinkedInMatchEmail", value); }
        }

        public LightContactsModel LinkedInContactsImport
        {
            get { return this.GetObject<LightContactsModel>("LinkedInContactsImport"); }
            set { this.Set<LightContactsModel>("LinkedInContactsImport", value); }
        }
    }
}