
namespace Sparkle.WebStatus.Controllers
{
    using LinkedInNET;
    using LinkedInNET.OAuth2;
    using Sparkle.Services.StatusApi;
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class RedirectController : Controller
    {
        const string FailReturnCookieName = "SpkStsLinUid";

        public ActionResult Index()
        {
            return this.ResultService.NotFound();
        }

        [HttpPost, Authorize]
        public ActionResult CreateLinkedInAuth(int userId, int scope, string apiKey, string returnUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
                return this.ResultService.JsonError("EmptyApiKey");
            if (string.IsNullOrEmpty(returnUrl))
                return this.ResultService.JsonError("EmptyReturnUrl");

            ////if (!this.ApiServices.IsKeyAuthorized(this.Request))
            ////    return this.ResultService.JsonError("UnauthorizedApiKey");

            var redirectId = Guid.NewGuid();
            this.Domain.LinkedInRedirections.Insert(
                redirectId,
                userId,
                scope,
                apiKey,
                Guid.NewGuid().ToString("N"),
                returnUrl);

            var go = "/Redirect/Go?redirectId=" + redirectId;
            var result = new LinkedInAuthorizationRedirectionCreate
            {
                GoUrl = UrlTools.Compose(this.Request) + go,
                LinkedInReturnUrl = this.GetLinkedInReturnUrl(),
            };

            return this.ResultService.JsonSuccess(result);
        }

        public ActionResult Go(Guid redirectId, string failReturnUrl)
        {
            var redirect = this.Domain.LinkedInRedirections.GetById(redirectId);
            if (redirect == null)
            {
                return this.Redirect(failReturnUrl);
            }

            ////if (!this.Request.Cookies.AllKeys.Contains(FailReturnCookieName))
            {
                var cookie = new HttpCookie(FailReturnCookieName, failReturnUrl);
                cookie.Expires = DateTime.UtcNow.AddYears(1);
                this.Response.Cookies.Add(cookie);
            }

            var returnUrlForLinkedIn = this.GetLinkedInReturnUrl();

            var apiConfig = new LinkedInApiConfiguration
            {
                ApiKey = redirect.ApiKey,
            };
            var linkedInApi = new LinkedInApi(apiConfig);
            var goUrl = linkedInApi.OAuth2.GetAuthorizationUrl(
                (AuthorizationScope)redirect.Scope,
                redirect.State,
                returnUrlForLinkedIn);

            Trace.WriteLine("RedirectController.Go: redirecting " + redirectId + " to " + goUrl);

            return this.Redirect(goUrl.AbsoluteUri);
        }

        public ActionResult LinkedInAuthorize(string code, string error, string error_description, string state)
        {
            string goUrl;

            if (!string.IsNullOrEmpty(error) || !string.IsNullOrEmpty(error_description))
            {
                Trace.TraceError("RedirectController.LinkedInAuthorize: ERROR '" + this.Request.Url.PathAndQuery + "'");
            }

            var redirect = this.Domain.LinkedInRedirections.GetByState(state);
            if (redirect == null)
            {
                if (this.Request.Cookies.AllKeys.Contains(FailReturnCookieName))
                {
                    goUrl = this.Request.Cookies[FailReturnCookieName].Value;

                    if (!string.IsNullOrEmpty(goUrl))
                    {
                        // put LI's error message as extra params
                        goUrl += "&li_error_message=" + Uri.EscapeDataString(error_description);
                        goUrl += "&li_error=" + Uri.EscapeDataString(error); ;
                    }
                    else
                    {
                        goUrl = "http://www.linkedin.com/";
                    }
                }
                else
                {
                    goUrl = "http://www.linkedin.com/";
                }

                Trace.WriteLine("RedirectController.LinkedInAuthorize: redirect==null. redirecting " + state + " to " + goUrl);
                return this.Redirect(goUrl);
            }

            var returnNwk = redirect.ReturnUrl;

            if (string.IsNullOrEmpty(code))
            {
                goUrl = returnNwk + string.Format("&error={0}&errorDesc={1}", error, error_description);
            }
            else
            {
                goUrl = returnNwk + string.Format("&code={0}&redirId={1}", code, redirect.Id);
            }

            Trace.WriteLine("RedirectController.LinkedInAuthorize: redirecting " + state + " to " + goUrl);
            return this.Redirect(goUrl);
        }

        [Authorize]
        public ActionResult GetLinkedInAuth(Guid guid)
        {
            ////if (!this.ApiServices.IsKeyAuthorized(this.Request))
            ////    return this.ResultService.JsonError("UnauthorizedApiKey");

            var redirect = this.Domain.LinkedInRedirections.GetById(guid);
            if (redirect == null)
                return this.ResultService.JsonError("RedirectionNotFound");

            var result = new LinkedInAuthorizationRedirectionData
            {
                UserId = redirect.UserId,
                PreviousReturnUrl = UrlTools.Compose(this.Request) + "/Redirect/LinkedInAuthorize",
            };
            return this.ResultService.JsonSuccess(result);
        }

        private string GetLinkedInReturnUrl()
        {
            var returnUrlForLinkedIn = UrlTools.Compose(this.Request) + "/Redirect/LinkedInAuthorize";
            return returnUrlForLinkedIn;
        }
    }
}
