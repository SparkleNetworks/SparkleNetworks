
namespace Sparkle.WebStatus.Controllers
{
    using Sparkle.WebStatus.Domain;
    using SrkToolkit.Web;
    using System;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AuthController : Controller
    {
        private const string identityPrefix = "Basic/";

        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult Off()
        {
            this.NavigationLine().Add("Off", this.Url.Action("Off"));

            if (this.User.Identity != null && this.User.Identity.Name != null)
            {
                FormsAuthentication.SignOut();
                this.TempData.AddConfirmation("Logged off.");
            }

            return this.RedirectToAction("Basic");
        }

        public ActionResult Basic()
        {
            this.NavigationLine().Add("Basic", this.Url.Action("Basic"));

            BasicUserModel user = null;
            if (this.User.Identity != null && this.User.Identity.Name != null)
            {
                var identity = this.User.Identity.Name;
                if (identity.StartsWith(identityPrefix))
                {
                    var id = identity.Substring(identityPrefix.Length);
                    Guid guid;
                    if (Guid.TryParse(id, out guid))
                    {
                        user = this.Services.BasicUsers.Get(guid);
                    }
                }
            }

            this.ViewBag.IsAuth = user != null;
            this.ViewBag.CurrentUser = user != null ? user.Guid.ToString() : default(string);

            if (this.Request.IsHttpPostRequest() && user == null)
            {
                var password = this.Request.Form["password"];
                if (!string.IsNullOrEmpty(password))
                {
                    Guid passwordId;
                    if (Guid.TryParse(password, out passwordId))
                    {
                        var result=this.Services.BasicUsers.Authenticate(passwordId);
                        if (result != null)
                        {
                            this.TempData.AddConfirmation("Authenticated.");
                            FormsAuthentication.SetAuthCookie(identityPrefix + result.Guid, false, "/");
                            return this.RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            this.TempData.AddError("Nop.");
                        }
                    }
                    else
                    {
                        this.TempData.AddError("Nop.");
                    }
                }
            }

            return this.View();
        }

        public ActionResult CreateDefaultBasicUser()
        {
            try
            {
                var result = this.Services.BasicUsers.CreateDefaultUser();
                this.TempData.AddConfirmation("Done.");
                return this.RedirectToAction("Basic");
            }
            catch (InvalidOperationException ex)
            {
                this.TempData.AddError(ex.Message);
            }

            return this.RedirectToAction("Index", "Home");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.NavigationLine().Add("Auth", this.Url.Action("Auth"));

            base.OnActionExecuting(filterContext);
        }
    }
}
