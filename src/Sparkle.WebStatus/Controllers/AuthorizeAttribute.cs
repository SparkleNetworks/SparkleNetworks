
namespace Sparkle.WebStatus.Controllers
{
    using Sparkle.Services.StatusApi;
    using Sparkle.WebStatus.Domain;
    using Sparkle.WebStatus.Services;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Custom authentication mecanism specific to this website. Works with asp authentication and API keys.
    /// </summary>
    public class AuthorizeAttribute : SrkToolkit.Web.Filters.AuthorizeAttribute
    {
        private bool authenticateWhenAnonymous = true;
        private bool authenticateWhenForbiden = false;

        public AuthorizeAttribute()
        {
            this.authenticateWhenAnonymous = false;
        }

        public bool AuthenticateWhenAnonymous
        {
            get { return this.authenticateWhenAnonymous; }
            set { this.authenticateWhenAnonymous = value; }
        }

        public bool AuthenticateWhenForbiden
        {
            get { return this.authenticateWhenForbiden; }
            set { this.authenticateWhenForbiden = value; }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (isAuthorized)
                return isAuthorized;

            var apiHeaders = new NetworkStatusApiKeyPayload(httpContext);
            if (apiHeaders.Key != null)
            {
                var services = httpContext.GetStatusLocalServices(false);
                var apiKey = services.ApiKeys.GetByKey(apiHeaders.Key);
                if (apiKey != null)
                {
                    var ok = apiHeaders.Verify(apiKey.Key, apiKey.Secret, DateTime.UtcNow);

                    if (!ok)
                    {
                        var codes = string.Join(", ", apiHeaders.VerificationErrors.Select(x => x.Key));
                        httpContext.Response.Headers.Add("X-SparkleStatus-ErrorCodes", codes);
                        Trace.WriteLine("Sparkle.WebStatus.Controllers.AuthorizeAttribute.AuthorizeCore: unauthorized call from " + httpContext.Request.UserHostAddress + " with key " + apiHeaders.Key + ": " + codes);
                    }

                    return ok;
                }
                else
                {
                    Trace.WriteLine("Sparkle.WebStatus.Controllers.AuthorizeAttribute.AuthorizeCore: unauthorized call from " + httpContext.Request.UserHostAddress + " with no key ");
                }
            }

            return false;
        }

        protected override SrkToolkit.Web.Services.IResultService GetResultService(HttpContextBase httpContext)
        {
            return new ResultService(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext != null && filterContext.HttpContext != null && filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity != null && filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (this.authenticateWhenForbiden)
                {
                    // if authenticated, invite to login using the default behavior
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    // if authenticated, display a forbidden page
                    filterContext.Result = new HttpStatusCodeResult(403, "Forbidden");
                    var resultService = this.GetResultService(filterContext.HttpContext);
                    resultService.Forbidden();
                }
            }
            else
            {
                if (this.authenticateWhenAnonymous)
                {
                    // if not authenticated, invite to login using the default behavior
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    // if not authenticated, display a forbidden page
                    filterContext.Result = new HttpStatusCodeResult(403, "Forbidden");
                    var resultService = this.GetResultService(filterContext.HttpContext);
                    resultService.Forbidden();
                }
            }
        }
    }
}