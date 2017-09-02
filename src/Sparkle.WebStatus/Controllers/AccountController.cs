
namespace Sparkle.WebStatus.Controllers
{
    using Sparkle.NetworksStatus.Domain.Messages;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AccountController : Controller
    {
        const string AuthTokenKeysKey = "AuthTokenKeys";

        public ActionResult Index()
        {
            return this.View();
        }

        private ActionResult Login()
        {
            var tokenKeys = (IList<string>)this.Session[AuthTokenKeysKey];
            this.ViewBag.TokenKeys = tokenKeys != null ? string.Join(";", tokenKeys) : string.Empty;
            this.ViewBag.RememberMe = false;

            var model = new EmailPasswordAuthenticateRequest();
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private ActionResult Login(string ReturnUrl, EmailPasswordAuthenticateRequest model, bool RememberMe = false)
        {
            model = model ?? new EmailPasswordAuthenticateRequest();
            this.ViewBag.RememberMe = RememberMe;
            this.ViewBag.ReturnUrl = ReturnUrl;

            model.RemoteAddress = this.Request.UserHostAddress;
            model.UserAgent = this.Request.UserAgent;

            if (this.ModelState.IsValid)
            {
                var result = this.Domain.Users.EmailPasswordAuthenticate(model);
                if (this.ValidateResult(result))
                {
                    FormsAuthentication.SetAuthCookie("$0$" + result.User.Id, RememberMe);
                    return this.RedirectToLocal(this.GetAnyLocalUrl(ReturnUrl, false, this.Url.Action("Index", "Home")));
                }
            }

            return this.View(model);
        }
    }
}
